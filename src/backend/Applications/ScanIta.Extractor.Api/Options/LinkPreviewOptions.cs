using System.ComponentModel.DataAnnotations;
using ValidateOrThrow;

namespace ScanIta.Crawler.Api.Options;

public sealed class LinkPreviewOptions : IValidatedOption
{
    public string SectionName => "LinkPreview";

    [Required]
    public required string ApiKey { get; set; }

}