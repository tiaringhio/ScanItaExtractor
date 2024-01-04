using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using ScanIta.Crawler.Api.Models;
using ScanIta.Crawler.Api.Constants;
using ILogger = Serilog.ILogger;

namespace ScanIta.Crawler.Api.Services.ImageExtraction;

public sealed partial class ImageExtractionService : IImageExtractionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly IMemoryCache _memoryCache;

    public ImageExtractionService(
        IHttpClientFactory httpClientFactory,
        ILogger logger,
        IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    private const int FirstPage = 2;
    
    public async Task<IEnumerable<ScanResult>> ExtractPagesAsync(string scanUrl, CancellationToken cts = default)
    {
        try
        {
            var pages = new List<ScanResult>();
            
            // number 1 is a presentation, will skip
            var currentPage = FirstPage;
            
            var urlWithoutPages = new Uri(scanUrl);
            
            if (!IsUrlInCorrectFormat(scanUrl))
            {
                var lastIndex = urlWithoutPages.AbsolutePath.LastIndexOf('/');
                scanUrl = urlWithoutPages.AbsolutePath[..lastIndex];
            }
            else
            {
                scanUrl = urlWithoutPages.AbsolutePath;
            }

            var nextPage = await ExtractPageAsync($"{SharedConstants.ScanItaBaseUrl}{scanUrl}/{currentPage}", isFirstPage: true, cancellationToken: cts);
            
            while (nextPage is {IsValidPage: true})
            {
                pages.Add(nextPage);
                currentPage++;
                nextPage = await ExtractPageAsync($"{SharedConstants.ScanItaBaseUrl}{scanUrl}/{currentPage}", cancellationToken: cts);
            }
            
            return pages;
            
        }
        catch (Exception e)
        {
            _logger.Error("Error while extracting pages: {Message}", e.Message);
        }

        return Enumerable.Empty<ScanResult>();
    }
    
    private async Task<ScanResult?> ExtractPageAsync(string scanUrl, bool isFirstPage = false, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_memoryCache.TryGetValue(scanUrl, out ScanResult? cachedPage))
            {
                if (!IsScanExpired(cachedPage!))
                    return cachedPage;
            }
            
            var scanId = new Uri(scanUrl);
            var client = _httpClientFactory.CreateClient(SharedConstants.ScanItaClientName);
        
            var response = await client.GetAsync(scanId.AbsolutePath, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;
        
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            
            var matches = ExtractImageRegex().Matches(body);
            
            var scan = matches
                .Select(x => x.Value)
                .FirstOrDefault();
        
            if (string.IsNullOrEmpty(scan))
                return null;
        
            // extract image url from img tag
            var imageUrl = scan
                .Split("\"")
                .Skip(1)
                .First();

            var result = new ScanResult();

            if (string.IsNullOrEmpty(imageUrl))
            {
                result.IsValidPage = false;
                return result;
            }

            if (isFirstPage)
            {
                result.ChapterName = ExtractChapterName(body);
            }
            result.PageUrl = imageUrl;
            result.IsValidPage = true;
            result.ExpirationTime = GetExpirationTime(imageUrl);
            
            _memoryCache.Set(scanUrl, result);
            return result;
        }
        catch (Exception e)
        {
            _logger.Error("Error while extracting page: {Message}", e.Message);
        }

        return null;
    }
     
    private static bool IsUrlInCorrectFormat(string url)
    {
        var lastIndex = url.LastIndexOf('/');
        return lastIndex != url.Length - 2;
    }
    
    private static string ExtractChapterName(string body)
    {
        var titleMatches = ExtractTitleRegex().Matches(body);
        var title = titleMatches
            .Select(x => x.Value)
            .FirstOrDefault();


        var titleComponents = title?
            .Replace("<title>", "")
            .Replace("</title>", "")
            .Trim()
            .Split("-");

        if (titleComponents is {Length: > 1})
        {
            return titleComponents[1].Trim() + " - " + titleComponents[0].Trim();
        }

        return string.Empty;
    }
    
    private static long GetExpirationTime(string scanUrl)
        => Convert.ToInt64(scanUrl
            .Split('&')
            .Last()
            .Split('=')
            .Last());
    
    private static bool IsScanExpired(ScanResult scan)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return now > scan.ExpirationTime;
    }

    [GeneratedRegex("""<img[^>]*class=['\"]img-fluid['\"][^>]*>""")]
    private static partial Regex ExtractImageRegex();
    
    [GeneratedRegex(@"<title>\s*(.*?)\s*</title>")]
    private static partial Regex ExtractTitleRegex();
    
    
}