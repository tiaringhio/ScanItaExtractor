using ScanIta.Crawler.Api.Models;

namespace ScanIta.Crawler.Api.Services.ImageExtraction;

public interface IImageExtractionService
{
    Task<IEnumerable<ScanResult>> ExtractPagesAsync(string scanUrl, CancellationToken cts = default);
}