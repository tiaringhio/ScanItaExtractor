# ScanIta Extractor

> WIP... infra will not deploy files to SWA, still figuring out a way to do it.

.NET 8 API and Angular 16 SPA to convert [Scan Ita](https://scanita.org) manga scans to PDF, one chapter at a time.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Ftiaringhio%2FScanItaExtractor%2Fmaster%2Finfrastructure%2Fmain.json)

You can deploy your own version to Azure by click the button above, here'e brief link of the components:

## DevOps

[![Api](https://github.com/tiaringhio/ScanItaExtractor/actions/workflows/api.yaml/badge.svg?branch=master)](https://github.com/tiaringhio/ScanItaExtractor/actions/workflows/api.yaml) [![Client](https://github.com/tiaringhio/ScanItaExtractor/actions/workflows/frontend.yaml/badge.svg?branch=master)](https://github.com/tiaringhio/ScanItaExtractor/actions/workflows/frontend.yaml) [![Infrastructure](https://github.com/tiaringhio/ScanItaExtractor/actions/workflows/infrastructure.yaml/badge.svg?branch=master)](https://github.com/tiaringhio/ScanItaExtractor/actions/workflows/infrastructure.yaml)

I'm using GitHub actions to deploy changes to the components of this application, i will gradually add information on how to replicate the DevOps configuration in order to have you very own infra available. This will probably mean that the Deplyo to Azure button will vanish in place of a more structured procedure.

## Azure Infra Components

- **Azure Container App Environment & Container App**: for the API, the image is hosted on this repo using ghcr. It's written to be scaled to 0 so you will use it (almost) only when you need it.
- **Azure Static Web App**: for the client, the code is stored in the frontend folder in this repo.
- **Log Analytics & Application Insights**: useful for monitoring purposes.

![Infrastructure Diagram](/files/infra_diagram.png)
> The image is auto-generated via bicep, it's an approximation.

## How does it work

### Frontend

You must have installed Node 18+ and Angular version 16+, i won't cover how to here.
Once deployed to Azure or cloned locally you run the frontend application by going to the [frontend folder](/src/frontend/scanitaextractor/) and run:

`npm install` then `ng serve`

when everything is done visit the url http://localhost:4200.

### Backend

You must have installed .NET 8 SDK, i won't cover how to install here.
> OPTIONAL: Populate you appsettings.json or better yet create a secrets file with the following structure:
```json
{
  "ConnectionStrings": {
    "ApplicationInsights": "connection string to an application insights instance"
  }
}
```

Go to the [api folder](/src/backend/Applications/ScanIta.Crawler.Api/) and run:

`dotnet run`

You can visit swagger (it only works locally) at the url https://localhost:7269

# Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create.
Any contributions you make are greatly appreciated.

1. Fork the Project
2. Create your Feature Branch (git checkout -b feature/AmazingFeature)
3. Commit your Changes (git commit -m 'Add some AmazingFeature')
4. Push to the Branch (git push origin feature/AmazingFeature)
5. Open a Pull Request