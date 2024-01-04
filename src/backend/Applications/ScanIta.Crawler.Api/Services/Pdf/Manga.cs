using System.Diagnostics;
using QuestPDF.Fluent;
using QuestPDF.Previewer;
using Image = SixLabors.ImageSharp.Image;

namespace ScanIta.Crawler.Api.Services.Pdf;

public sealed class Manga
{
    public static async Task<byte[]> GeneratePdf(IEnumerable<string?> imageUrls)
    {
        var client = new HttpClient();
        var pages = new List<MangaPage>();
        
        foreach (var url in imageUrls)
        {
            var image = await client.GetByteArrayAsync(url);
            using var imageData = Image.Load(image);
            var width = imageData.Width;
            var height = imageData.Height;
            pages.Add(new MangaPage
            {
                Data = image,
                Width = width,
                Height = height
            });
        }

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
        
        if (Debugger.IsAttached)
            await document.ShowInPreviewerAsync();
        
        return document.GeneratePdf();
    }

    private sealed record MangaPage
    {
        public required byte[] Data { get; init; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}