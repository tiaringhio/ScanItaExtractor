using Microsoft.AspNetCore.Mvc;
using ScanIta.Crawler.Api.Services.ImageExtraction;
using ScanIta.Crawler.Api.Services.Pdf;

namespace ScanIta.Crawler.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public sealed class EtractorController : ControllerBase
{
    private readonly IImageExtractionService _imageExtractionService;

    public EtractorController(IImageExtractionService imageExtractionService)
    {
        _imageExtractionService = imageExtractionService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string scanUrl)
    {
        var imageUrl = await _imageExtractionService.ExtractPageAsync(scanUrl);

        if (string.IsNullOrEmpty(imageUrl.PageUrl))
            return NotFound();

        return Ok(imageUrl);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMultiple([FromQuery] string scanUrl)
    {
        var imageUrls = await _imageExtractionService.ExtractPagesAsync(scanUrl);

        if (imageUrls.Count == 0)
            return NotFound();
        
        var pdf = await Manga.GeneratePdf(imageUrls);
        
        return File(pdf, "application/pdf", "manga.pdf");
    }

    
}