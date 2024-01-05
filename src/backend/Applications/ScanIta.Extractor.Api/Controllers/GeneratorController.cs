using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ScanIta.Crawler.Api.Constants;
using ScanIta.Crawler.Api.Models;
using ScanIta.Crawler.Api.Options;
using ScanIta.Crawler.Api.Services.ImageExtraction;
using ScanIta.Crawler.Api.Services.Pdf;

namespace ScanIta.Crawler.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public sealed class GeneratorController : ControllerBase
{
    private readonly IImageExtractionService _imageExtractionService;
    private readonly IMangaService _mangaService;
    private readonly LinkPreviewOptions _linkPreviewOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    
    public GeneratorController(
        IImageExtractionService imageExtractionService,
        IMangaService mangaService,
        IOptions<LinkPreviewOptions> linkPreviewOptions,
        IHttpClientFactory httpClientFactory)
    {
        _imageExtractionService = imageExtractionService;
        _mangaService = mangaService;
        _httpClientFactory = httpClientFactory;
        _linkPreviewOptions = linkPreviewOptions.Value;
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
    public async Task<IActionResult> PreviewLink([FromQuery] string link)
    {
        var client = _httpClientFactory.CreateClient(SharedConstants.LinkPreviewBaseUrl);
        
        var response = await client.GetAsync($"?q={link}");
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            return StatusCode(429);
        }
        if (!response.IsSuccessStatusCode)
        {
            return BadRequest();
        }
        var content = await response.Content.ReadFromJsonAsync<PreviewLinkResult>();
        
        return Ok(content);
    }
}