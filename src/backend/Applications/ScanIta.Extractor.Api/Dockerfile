FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/backend/Applications/ScanIta.Extractor.Api/ScanIta.Extractor.Api.csproj", "src/backend/Applications/ScanIta.Extractor.Api/"]
RUN dotnet restore "src/backend/Applications/ScanIta.Extractor.Api/ScanIta.Extractor.Api.csproj"
COPY . .
WORKDIR "/src/src/backend/Applications/ScanIta.Extractor.Api"
RUN dotnet build "ScanIta.Extractor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ScanIta.Extractor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScanIta.Extractor.Api.dll"]
