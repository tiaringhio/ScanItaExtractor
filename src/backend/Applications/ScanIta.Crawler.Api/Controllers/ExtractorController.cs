using Microsoft.AspNetCore.Mvc;
using ScanIta.Crawler.Api.Models;
using ScanIta.Crawler.Api.Services.ImageExtraction;
using ScanIta.Crawler.Api.Services.Pdf;

namespace ScanIta.Crawler.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public sealed class ExtractorController(IImageExtractionService imageExtractionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMultiple([FromQuery] string scanUrl)
    {
        var imageUrls = await imageExtractionService.ExtractPagesAsync(scanUrl);

        var scanResults = imageUrls as ScanResult[] ?? imageUrls.ToArray();
        
        if (scanResults.Length == 0)
            return NotFound();
        
        var pdf = await Manga.GeneratePdf(scanResults.Select(x => x.PageUrl));
        
        var title = scanResults.First().ChapterName;
        return File(pdf, "application/pdf",$"{title}.pdf");
    }
}