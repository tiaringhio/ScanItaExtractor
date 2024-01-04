using ScanIta.Crawler.Api.Models;

namespace ScanIta.Crawler.Api.Services;

public interface IImageExtractionService
{
    Task<ScanResult?> ExtractPageAsync(string scanUrl);
    Task<IList<string>> ExtractPagesAsync(string scanUrl);
}