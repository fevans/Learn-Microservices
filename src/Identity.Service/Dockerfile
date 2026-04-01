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

# Restore Identity.Service with local packages
COPY ["Identity.Service/Identity.Service.csproj", "Identity.Service/"]
RUN dotnet restore "Identity.Service/Identity.Service.csproj" --source /packages --source https://api.nuget.org/v3/index.json

# Build Identity.Service
COPY ["Identity.Service/", "Identity.Service/"]
WORKDIR "/src/Identity.Service"
RUN dotnet build "Identity.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Identity.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Service.dll"]
