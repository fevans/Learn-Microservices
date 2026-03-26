FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy and pack GamePlatform.Common
COPY ["GamePlatform.Common/", "GamePlatform.Common/"]
RUN dotnet pack "GamePlatform.Common/GamePlatform.Common.csproj" -c Release -o /packages

# Copy and pack GamePlatform.Catalog.Contracts
COPY ["GamePlatform.Catalog.Contracts/", "GamePlatform.Catalog.Contracts/"]
RUN dotnet pack "GamePlatform.Catalog.Contracts/GamePlatform.Catalog.Contracts.csproj" -c Release -o /packages

# Restore Inventory.Service with local packages
COPY ["Inventory.Service/Inventory.Service.csproj", "Inventory.Service/"]
RUN dotnet restore "Inventory.Service/Inventory.Service.csproj" --source /packages --source https://api.nuget.org/v3/index.json

# Build Inventory.Service
COPY ["Inventory.Service/", "Inventory.Service/"]
WORKDIR "/src/Inventory.Service"
RUN dotnet build "Inventory.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Inventory.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventory.Service.dll"]

