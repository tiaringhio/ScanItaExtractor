using System.Text.RegularExpressions;
using ScanIta.Crawler.Api.Models;
using ScanIta.Crawler.Api.Constants;

namespace ScanIta.Crawler.Api.Services.ImageExtraction;

public sealed partial class ImageExtractionService(IHttpClientFactory httpClientFactory, ILogger<ImageExtractionService> logger) : IImageExtractionService
{
    private const int FirstPage = 2;
    
    public async Task<IEnumerable<ScanResult>> ExtractPagesAsync(string scanUrl)
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

            var nextPage = await ExtractPageAsync($"{SharedConstants.ScanItaBaseUrl}{scanUrl}/{currentPage}", isFirstPage: true);
            
            while (nextPage is {IsValidPage: true})
            {
                pages.Add(nextPage);
                currentPage++;
                nextPage = await ExtractPageAsync($"{SharedConstants.ScanItaBaseUrl}{scanUrl}/{currentPage}");
            }
            
            return pages;
            
        }
        catch (Exception e)
        {
            logger.LogError("Error while extracting pages: {Message}", e.Message);
        }

        return Enumerable.Empty<ScanResult>();
    }
    
    private static bool IsUrlInCorrectFormat(string url)
    {
        var lastIndex = url.LastIndexOf('/');
        return lastIndex != url.Length - 2;
    }
    
    private async Task<ScanResult?> ExtractPageAsync(string scanUrl, bool isFirstPage = false)
    {
        try
        {
            var scanId = new Uri(scanUrl);
            var client = httpClientFactory.CreateClient(SharedConstants.ScanItaClientName);
        
            var response = await client.GetAsync(scanId.AbsolutePath);

            if (!response.IsSuccessStatusCode)
                return null;
        
            var body = await response.Content.ReadAsStringAsync();
            
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
            return result;
        }
        catch (Exception e)
        {
            logger.LogError("Error while extracting page: {Message}", e.Message);
        }

        return null;
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

    [GeneratedRegex("""<img[^>]*class=['\"]img-fluid['\"][^>]*>""")]
    private static partial Regex ExtractImageRegex();
    
    [GeneratedRegex(@"<title>\s*(.*?)\s*</title>")]
    private static partial Regex ExtractTitleRegex();
    
    
}