using ScanIta.Crawler.Api.Constants;
using ScanIta.Crawler.Api.Services.ImageExtraction;
using ScanIta.Crawler.Api.Services.Pdf;

namespace ScanIta.Crawler.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void HttpClients(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient(SharedConstants.ScanItaClientName, client =>
        {
            client.BaseAddress = new Uri(SharedConstants.ScanItaBaseUrl);
        });
        
        var linkPreviewApiKey = configuration.GetSection("LinkPreview:ApiKey").Value;
        services.AddHttpClient(SharedConstants.LinkPreviewBaseUrl, client =>
        {
            client.BaseAddress = new Uri(SharedConstants.LinkPreviewBaseUrl);
            client.DefaultRequestHeaders.Add("X-Linkpreview-Api-Key", linkPreviewApiKey);
        });
    }
    
    public static void AddBusiness(this IServiceCollection services)
    {
        services.AddScoped<IImageExtractionService, ImageExtractionService>();
        services.AddScoped<IMangaService, MangaService>();
    }
}