# Stage 1: Base Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first (for better caching)
COPY ["DZ_MP_SYSTEM_MGMT_SERVICE.API/DZ_MP_SYSTEM_MGMT_SERVICE.API.csproj", "DZ_MP_SYSTEM_MGMT_SERVICE.API/"]
COPY ["DZ_MP.CORE/DZ_MP.CORE.csproj", "DZ_MP.CORE/"]

# Restore dependencies
RUN dotnet restore "DZ_MP_SYSTEM_MGMT_SERVICE.API/DZ_MP_SYSTEM_MGMT_SERVICE.API.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/DZ_MP_SYSTEM_MGMT_SERVICE.API"

# Publish
RUN dotnet publish "DZ_MP_SYSTEM_MGMT_SERVICE.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Final Image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DZ_MP_SYSTEM_MGMT_SERVICE.API.dll"]
