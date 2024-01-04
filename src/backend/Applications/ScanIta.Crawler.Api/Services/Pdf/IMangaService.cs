namespace ScanIta.Crawler.Api.Services.Pdf;

public interface IMangaService
{
    Task<byte[]> GeneratePdf(IEnumerable<string> imageUrls, CancellationToken cancellationToken = default);
}