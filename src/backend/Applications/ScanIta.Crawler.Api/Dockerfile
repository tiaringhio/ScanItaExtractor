FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/backend/Applications/ScanIta.Crawler.Api/ScanIta.Crawler.Api.csproj", "src/backend/Applications/ScanIta.Crawler.Api/"]
RUN dotnet restore "src/backend/Applications/ScanIta.Crawler.Api/ScanIta.Crawler.Api.csproj"
COPY . .
WORKDIR "/src/src/backend/Applications/ScanIta.Crawler.Api"
RUN dotnet build "ScanIta.Crawler.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ScanIta.Crawler.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScanIta.Crawler.Api.dll"]
