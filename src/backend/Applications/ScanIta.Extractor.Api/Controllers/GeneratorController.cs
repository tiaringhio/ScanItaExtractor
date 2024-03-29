using Microsoft.AspNetCore.Mvc;
using ScanIta.Crawler.Api.Models;
using ScanIta.Crawler.Api.Services.ImageExtraction;
using ScanIta.Crawler.Api.Services.Pdf;
using ScanIta.Crawler.Api.Services.Preview;

namespace ScanIta.Crawler.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public sealed class GeneratorController : ControllerBase
{
    private readonly IImageExtractionService _imageExtractionService;
    private readonly IMangaService _mangaService;
    private readonly IPreviewService _previewService;
    public GeneratorController(
        IImageExtractionService imageExtractionService,
        IMangaService mangaService,
        IPreviewService previewService)
    {
        _imageExtractionService = imageExtractionService;
        _mangaService = mangaService;
        _previewService = previewService;
    }

    [HttpGet]
    public async Task<IActionResult> GeneratePdf([FromQuery] string scanUrl,
        CancellationToken cts = default)
    {
        try
        {
            var imageUrls = await _imageExtractionService.ExtractPagesAsync(scanUrl, cts);

            var scanResults = imageUrls as ScanResult[] ?? imageUrls.ToArray();
        
            if (scanResults.Length == 0)
                return NotFound();
        
            var pdf = await _mangaService.GeneratePdf(scanResults.Select(x => x.PageUrl)!, cts);
        
            var title = scanResults.First().ChapterName;

            return File(pdf, "application/pdf",$"{title}.pdf");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> PreviewLink([FromQuery] string link, CancellationToken cts = default)
    {
        var previewLink = await _previewService.GetPreviewLinkAsync(link, cts);
        if (previewLink == null)
            return NotFound();
        return Ok(previewLink);
    }
}