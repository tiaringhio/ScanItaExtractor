using System.Text.RegularExpressions;
using ScanIta.Crawler.Api.Models;

namespace ScanIta.Crawler.Api.Services;

public sealed partial class ImageExtractionService : IImageExtractionService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ImageExtractionService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ScanResult?> ExtractPageAsync(string scanUrl)
    {
        try
        {
            var regex = ExtractImageRegex();
            var scanId = new Uri(scanUrl);
            var client = _httpClientFactory.CreateClient("ScanIta");
        
            var response = await client.GetAsync(scanId.AbsolutePath);

            if (!response.IsSuccessStatusCode)
                return null;
        
            var body = await response.Content.ReadAsStringAsync();
        
            var matches = regex.Matches(body);
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
            
            result.PageUrl = imageUrl;
            result.IsValidPage = true;
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IList<string>> ExtractPagesAsync(string scanUrl)
    {
        try
        {
            var pages = new List<string>();
            // number 1 is a presentation, will skip
            var currentPage = 2;
            
            var urlWithoutPages = new Uri(scanUrl);
            
            if (!IsUrlInCorrectFormat(scanUrl))
            {
                var lastIndex = urlWithoutPages.AbsolutePath.LastIndexOf('/');
                scanUrl = urlWithoutPages.AbsolutePath[..lastIndex];
            }

            var nextPage = await ExtractPageAsync($"{scanUrl}/{currentPage}");
            
            while (nextPage is {IsValidPage: true})
            {
                pages.Add(nextPage.PageUrl!);
                currentPage++;
                nextPage = await ExtractPageAsync($"https://scanita.org{scanUrl}/{currentPage}");
            }
            
            return pages;
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private static bool IsUrlInCorrectFormat(string url)
    {
        var lastIndex = url.LastIndexOf('/');
        return lastIndex != url.Length - 2;
    }

    [GeneratedRegex("""<img[^>]*class=['\"]img-fluid['\"][^>]*>""")]
    private static partial Regex ExtractImageRegex();
}