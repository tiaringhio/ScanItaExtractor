using ScanIta.Crawler.Api.Models;

namespace ScanIta.Crawler.Api.Services.Preview;

public interface IPreviewService
{
    Task<PreviewLinkResult?> GetPreviewLinkAsync(string link, CancellationToken cts = default);
}