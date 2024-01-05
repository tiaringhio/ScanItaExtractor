# ScanIta Crawler

.NET 8 API and Angular 16 SPA to convert [Scan Ita](https://scanita.org) manga scans to PDF, one chapter at a time.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/)

You can deploy your own version to Azure by click the button above, here'e brief link of the components:

## Azure Infra Components

- Azure Container App Environment & Container App: for the API, the image is hosted on this repo using ghcr. It's written to be scaled to 0 so you will use it (almost) only when you need it.
- Azure Static Web App: for the client, the code is stored in the frontend folder in this repo.
- Log Analytics & Application Insights: usdeful for monitoring purposes

## How does it work

### Frontend

You must have installed Node 18+ and Angular version 16+, i won't cover how to here.
Once deployed to Azure or cloned locally you run the frontend application by going to the [frontend folder](/src/frontend/scanitaextractor/) and run:

`npm install && ng serve`

when everything is done visit the url http://localhost:4200.

### Backend

You must have installed .NET 8 SDK, i won't cover how to install here.
Go to the [api folder](/src/backend/Applications/ScanIta.Crawler.Api/) and run:

`dotnet run`

You can visit swagger (it only works locally) at the url https://localhost:7269

## Contribute

Fork the repo