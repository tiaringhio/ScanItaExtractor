using ScanIta.Crawler.Api.Constants;
using ScanIta.Crawler.Api.Services.ImageExtraction;
using ScanIta.Crawler.Api.Services.Pdf;

namespace ScanIta.Crawler.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddScanItaHttpClient(this IServiceCollection services)
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
    }
}