using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using QuestPDF.Fluent;
using QuestPDF.Previewer;
using Image = SixLabors.ImageSharp.Image;
using ILogger = Serilog.ILogger;

namespace ScanIta.Crawler.Api.Services.Pdf;

public sealed class MangaService : IMangaService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger _logger;
    public MangaService(
        IMemoryCache cache,
        ILogger logger)
    {
        _cache = cache;
        _logger = logger;
        if (Debugger.IsAttached)
        {
            QuestPDF.Settings.EnableDebugging = true;
        }
        
        QuestPDF.Settings.EnableCaching = true;
    }
    public async Task<byte[]> GeneratePdf(IEnumerable<string> imageUrls, CancellationToken cancellationToken = default)
    {
        
        var client = new HttpClient();
        var pages = new List<MangaPage>();

        var stopwatch = Stopwatch.StartNew();
        foreach (var url in imageUrls)
        {
            if (_cache.TryGetValue(url, out MangaPage cachedPage))
            {
                pages.Add(cachedPage!);
                continue;
            }
            
            var image = await client.GetByteArrayAsync(url, cancellationToken);
            using var imageData = Image.Load(image);
            var width = imageData.Width;
            var height = imageData.Height;
            pages.Add(new MangaPage(image, width, height));
            
            _cache.Set(url, image, TimeSpan.FromMinutes(5));
        }

        var imageRetrieval = stopwatch.Elapsed;
        _logger.Debug("Image retrieval took {ImageRetrieval}", imageRetrieval);
        
        var document = Document.Create(container =>
        {
            foreach (var mangaPage in pages)
            {
                var page1 = mangaPage;
                container.Page(page =>
                {
                    // if scan is horizontal, rotate it
                    if (page1.Width > page1.Height)
                    {
                        page
                            .Content()
                            .RotateLeft()
                            .Image(page1.Data)
                            .FitUnproportionally();
                    }
                    else
                    {
                        page
                            .Content()
                            .Image(page1.Data)
                            .FitUnproportionally();
                    }
                });
            }
        });
        
        var documentCreation = stopwatch.Elapsed - imageRetrieval;
        _logger.Debug("Document creation took {DocumentCreation}", documentCreation);
        
        if (Debugger.IsAttached)
            await document.ShowInPreviewerAsync(cancellationToken: cancellationToken);
        
        var result = document.GeneratePdf();
        stopwatch.Stop();
        var pdfGeneration = stopwatch.Elapsed - documentCreation;
        
        _logger.Debug("Pdf generation took {PdfGeneration}", pdfGeneration);
        
        return result;
    }

    private sealed record MangaPage(byte[] Data, int Width, int Height);
}