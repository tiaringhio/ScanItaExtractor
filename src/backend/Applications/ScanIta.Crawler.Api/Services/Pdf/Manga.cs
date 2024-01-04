using System.Diagnostics;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
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
            var aspectRatios = pages
                .Select(x => Math.Round((double)x.Width / x.Height, 2) * 1000)
                .ToList();
            
            var mostCommonAspectRatio = aspectRatios
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .First()
                .Key;
            
            for (var index = 0; index < pages.Count; index++)
            {
                var mangaPage = pages[index];
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    // if scan is horizontal, rotate it
                    if (mangaPage.Width > mangaPage.Height)
                    {
                        page
                            .Content()
                            .RotateLeft()
                            .Image(mangaPage.Data)
                            .FitUnproportionally();
                    }
                    else
                    {
                        // if scan has different aspect ratio, fit it to the most common one
                        if (aspectRatios[index] != mostCommonAspectRatio)
                        {
                            page
                                .Content()
                                .Image(mangaPage.Data)
                                .FitUnproportionally();
                        }
                        else
                        {
                            page
                                .Content()
                                .Image(mangaPage.Data);
                        }
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