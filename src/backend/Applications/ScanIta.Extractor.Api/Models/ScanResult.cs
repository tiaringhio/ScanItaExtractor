using System.Text.Json.Serialization;

namespace ScanIta.Crawler.Api.Models;

public sealed class ScanResult
{
    [JsonPropertyName("chapterName")]
    public string? ChapterName { get; set; }
    [JsonPropertyName("pageUrl")]
    public string? PageUrl { get; set; }
    [JsonPropertyName("isValidPage")]
    public bool IsValidPage { get; set; }
    [JsonPropertyName("expirationTime")]
    public long ExpirationTime { get; set; }
}