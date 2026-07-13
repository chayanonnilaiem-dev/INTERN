using DZ_MP.CORE.Extensions;
using DZ_MP.CORE.Middlewares;
using DZ_MP_BIZ_SERVICE.API.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// --- 1. Services Configuration ---
builder.Host.AddSerilogLogging(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddCustomerApiValidatorController();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScalarOpenApi("DZ-MP Biz Service API");
builder.Services.AddCustomRouting();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration["MP_DB_CONN"]
        ?? builder.Configuration.GetConnectionString("DefaultConnection")!
    )
);

builder.Services.AddHttpContextAccessor();

// Dependency Injection using Scrutor
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.InNamespaces(
        "DZ_MP_BIZ_SERVICE.API.Repositories.Impl",
        "DZ_MP_BIZ_SERVICE.API.Services.Impl"))
    .AsMatchingInterface()
    .WithScopedLifetime()
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    _ = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/scalar", options =>
    {
        options.Title = "DZ-MP Biz Service API";
    });
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new { service = "biz-service", status = "running" }))
    .ExcludeFromDescription();
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }))
    .ExcludeFromDescription();

app.Run();
