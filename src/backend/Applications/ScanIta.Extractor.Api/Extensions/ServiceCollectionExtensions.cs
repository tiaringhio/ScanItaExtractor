using ScanIta.Crawler.Api.Constants;
using ScanIta.Crawler.Api.Services.ImageExtraction;
using ScanIta.Crawler.Api.Services.Pdf;
using ScanIta.Crawler.Api.Services.Preview;

namespace ScanIta.Crawler.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void HttpClients(this IServiceCollection services)
    {
        services.AddHttpClient(SharedConstants.ScanItaClientName, client =>
        {
            client.BaseAddress = new Uri(SharedConstants.ScanItaBaseUrl);
        });
    }
    
    public static void AddBusiness(this IServiceCollection services)
    {
        services.AddScoped<IImageExtractionService, ImageExtractionService>();
        services.AddScoped<IMangaService, MangaService>();
        services.AddScoped<IPreviewService, PreviewService>();
    }
}