using System.Text.Json.Serialization;

namespace ScanIta.Crawler.Api.Models;

public sealed class PreviewLinkResult
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}