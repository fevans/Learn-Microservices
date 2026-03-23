FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Catalog.Service/Catalog.Service.csproj", "Catalog.Service/"]
COPY ["GamePlatform.Common/bin/Debug/GamePlatform.Common.1.0.1.nupkg", "/packages/"]
RUN dotnet restore "Catalog.Service/Catalog.Service.csproj" --source /packages --source https://api.nuget.org/v3/index.json
COPY ["Catalog.Service/", "Catalog.Service/"]
WORKDIR "/src/Catalog.Service"
RUN dotnet build "Catalog.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Catalog.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Service.dll"]
