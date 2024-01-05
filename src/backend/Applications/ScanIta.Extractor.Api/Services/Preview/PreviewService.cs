using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using ScanIta.Crawler.Api.Constants;
using ScanIta.Crawler.Api.Models;
using ILogger = Serilog.ILogger;

namespace ScanIta.Crawler.Api.Services.Preview;

public sealed partial class PreviewService : IPreviewService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger _logger;

    public PreviewService(
        IHttpClientFactory httpClientFactory,
        IMemoryCache memoryCache,
        ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<PreviewLinkResult?> GetPreviewLinkAsync(string link, CancellationToken cts = default)
    {
        try
        {
            if (_memoryCache.TryGetValue(link, out PreviewLinkResult? previewLink))
                return previewLink;
            
            var client = _httpClientFactory.CreateClient(SharedConstants.ScanItaClientName);
            
            var response = await client.GetAsync(link, cts);
            
            var content = await response.Content.ReadAsStringAsync(cts);
 
            var result = new PreviewLinkResult
            {
                Title = TitleRegex().Match(content).Groups[1].Value,
                Image = ImageRegex().Match(content).Groups[1].Value,
                Description =  DescriptionRegex().Match(content).Groups[1].Value,
                Url = link
            };
            
            _memoryCache.Set(link, result, TimeSpan.FromMinutes(10));
            
            return result;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while getting preview link");
        }

        return null;
    }

    [GeneratedRegex("<meta\\s+property\\s*=\\s*\"og:title\"\\s+content\\s*=\\s*\"(.*?)\"\\s*/?>")]
    private static partial Regex TitleRegex();
    
    [GeneratedRegex("<meta\\s+property\\s*=\\s*\"og:image\"\\s+content\\s*=\\s*\"(.*?)\"\\s*/?>")]
    private static partial Regex ImageRegex();
    
    [GeneratedRegex("<meta\\s+property\\s*=\\s*\"og:description\"\\s+content\\s*=\\s*\"(.*?)\"\\s*/?>")]
    private static partial Regex DescriptionRegex();
}