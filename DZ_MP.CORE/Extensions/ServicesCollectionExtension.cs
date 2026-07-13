using System.Reflection;
using System.Text;
using System.Xml.XPath;
using DZ_MP.CORE.Commons;
using DZ_MP.CORE.Models.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;

namespace DZ_MP.CORE.Extensions;

public static class ServicesCollectionExtension
{
    public static void AddSerilogLogging(this ConfigureHostBuilder host, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .Enrich.FromLogContext()
                        .Enrich.WithMachineName()
                        .Enrich.WithThreadId()
                        .Enrich.WithEnvironmentName()
                        .WriteTo.Console(outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj} <{SourceContext}> (Thread:{ThreadId} Machine:{MachineName}){NewLine}{Exception}")
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();

        host.UseSerilog();
    }

    public static IServiceCollection AddCustomerApiValidatorController(this IServiceCollection services)
    {
        services.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(x => new FieldErrorDetail(
                            x.Key,
                            x.Value!.Errors.First().ErrorMessage
                    )).ToList();

                var traceId = context.HttpContext.Items["CorrelationId"]?.ToString()
                            ?? context.HttpContext.TraceIdentifier;

                var response = BaseResponse<object>.Failure(
                    System.Net.HttpStatusCode.BadRequest,
                    ErrorCode.VALIDATEERROR,
                    "One or more validation errors occurred.",
                    traceId,
                    errors
                );

                return new BadRequestObjectResult(response);
            };
        });

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["JwtSettings:Key"];
        if (string.IsNullOrEmpty(secretKey)) return services;

        var key = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // Authorization header ที่ client ตั้งใจส่งมา ต้องชนะ cookie
                    if (!context.Request.Headers.ContainsKey("Authorization")
                        && context.Request.Cookies.ContainsKey("access_token"))
                    {
                        context.Token = context.Request.Cookies["access_token"];
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var traceId = context.HttpContext.Items["CorrelationId"]?.ToString()
                                  ?? context.HttpContext.TraceIdentifier;

                    logger.LogWarning("Unauthorized access: {Error}, Description: {Description}, TraceId: {TraceId}",
                                        context.Error ?? "No Token",
                                        context.ErrorDescription ?? "No authentication header provided",
                                        traceId);

                    var response = BaseResponse<object>.Failure(
                        System.Net.HttpStatusCode.Unauthorized,
                        ErrorCode.UNAUTHORIZED,
                        "Access is denied due to invalid credentials.",
                        traceId
                    );

                    await context.Response.WriteAsJsonAsync(response);
                },

                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var traceId = context.HttpContext.Items["CorrelationId"]?.ToString()
                                  ?? context.HttpContext.TraceIdentifier;

                    var response = BaseResponse<object>.Failure(
                        System.Net.HttpStatusCode.Forbidden,
                        ErrorCode.FORBIDDEN,
                        "You do not have permission to access this resource.",
                        traceId
                    );

                    await context.Response.WriteAsJsonAsync(response);
                }
            };

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    public static IServiceCollection AddScalarOpenApi(this IServiceCollection services, string title = "DZ-MP API")
    {
        var xmlNavigators = new List<XPathNavigator>();

        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
        {
            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{entryAssembly.GetName().Name}.xml");
            if (File.Exists(xmlPath))
                xmlNavigators.Add(new XPathDocument(xmlPath).CreateNavigator());
        }

        var coreXmlPath = Path.Combine(AppContext.BaseDirectory, "DZ_MP.CORE.xml");
        if (File.Exists(coreXmlPath))
            xmlNavigators.Add(new XPathDocument(coreXmlPath).CreateNavigator());

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, ct) =>
            {
                document.Info.Title = title;
                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, ct) =>
            {
                if (xmlNavigators.Count == 0) return Task.CompletedTask;
                if (context.Description.ActionDescriptor is not ControllerActionDescriptor cad) return Task.CompletedTask;

                var method = cad.MethodInfo;
                var paramList = string.Join(",", method.GetParameters().Select(p => GetXmlDocTypeName(p.ParameterType)));
                var xmlKey = paramList.Length > 0
                    ? $"M:{method.DeclaringType!.FullName}.{method.Name}({paramList})"
                    : $"M:{method.DeclaringType!.FullName}.{method.Name}";

                foreach (var nav in xmlNavigators)
                {
                    var summaryNode = nav.SelectSingleNode($"/doc/members/member[@name='{xmlKey}']/summary");
                    if (summaryNode != null)
                    {
                        operation.Summary = summaryNode.InnerXml.Trim();
                        break;
                    }
                }

                return Task.CompletedTask;
            });

            // แปลงชื่อ query parameter ทุกตัวเป็น camelCase ให้สอดคล้องกับ JSON body
            options.AddOperationTransformer((operation, context, ct) =>
            {
                if (operation.Parameters is not null)
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        if (parameter is OpenApiParameter p
                            && p.In == ParameterLocation.Query
                            && !string.IsNullOrEmpty(p.Name)
                            && char.IsUpper(p.Name[0]))
                        {
                            p.Name = char.ToLowerInvariant(p.Name[0]) + p.Name[1..];
                        }
                    }
                }

                return Task.CompletedTask;
            });

            // Fix type mismatches in .NET 10 OpenAPI schema generation
            options.AddSchemaTransformer((schema, context, ct) =>
            {
                var type = context.JsonTypeInfo.Type;
                var underlying = Nullable.GetUnderlyingType(type) ?? type;
                var hasAnyOf = schema.AnyOf is { Count: > 0 };

                if (underlying == typeof(DateOnly) && !hasAnyOf)
                {
                    schema.Type = JsonSchemaType.String;
                    schema.Format = "date";
                    schema.Properties?.Clear();
                    schema.Example = System.Text.Json.Nodes.JsonValue.Create("2025-01-01");
                    return Task.CompletedTask;
                }

                if (!hasAnyOf)
                {
                    if (underlying == typeof(int)) { schema.Type = JsonSchemaType.Integer; schema.Format = "int32"; }
                    else if (underlying == typeof(long)) { schema.Type = JsonSchemaType.Integer; schema.Format = "int64"; }
                    else if (underlying == typeof(short)) { schema.Type = JsonSchemaType.Integer; schema.Format = "int16"; }
                    else if (underlying == typeof(byte)) { schema.Type = JsonSchemaType.Integer; schema.Format = null; }
                    else if (underlying == typeof(decimal)) { schema.Type = JsonSchemaType.Number; schema.Format = "decimal"; }
                    else if (underlying == typeof(float)) { schema.Type = JsonSchemaType.Number; schema.Format = "float"; }
                    else if (underlying == typeof(double)) { schema.Type = JsonSchemaType.Number; schema.Format = "double"; }
                }

                if (schema.Type == JsonSchemaType.Array && type.IsGenericType)
                {
                    var elemType = type.GetGenericArguments().FirstOrDefault();
                    var elemUnderlying = Nullable.GetUnderlyingType(elemType ?? typeof(object)) ?? elemType;

                    if (elemUnderlying == typeof(int))
                        schema.Items = new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32" };
                    else if (elemUnderlying == typeof(long))
                        schema.Items = new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64" };
                    else if (elemUnderlying == typeof(short))
                        schema.Items = new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int16" };
                    else if (elemUnderlying == typeof(decimal))
                        schema.Items = new OpenApiSchema { Type = JsonSchemaType.Number, Format = "decimal" };
                    else if (elemUnderlying == typeof(float))
                        schema.Items = new OpenApiSchema { Type = JsonSchemaType.Number, Format = "float" };
                    else if (elemUnderlying == typeof(double))
                        schema.Items = new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double" };
                }

                return Task.CompletedTask;
            });
        });
        return services;
    }

    public static IServiceCollection AddCustomRouting(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });
        return services;
    }

    // แปลง CLR type name → XML doc format เช่น Nullable`1[[System.Int32,...]] → System.Nullable{System.Int32}
    private static string GetXmlDocTypeName(Type type)
    {
        if (!type.IsGenericType)
            return type.FullName ?? type.Name;

        var genericDef = type.GetGenericTypeDefinition();
        var defName = genericDef.FullName ?? genericDef.Name;
        var backtick = defName.IndexOf('`');
        if (backtick >= 0)
            defName = defName[..backtick];

        var args = string.Join(",", type.GetGenericArguments().Select(GetXmlDocTypeName));
        return $"{defName}{{{args}}}";
    }
}
