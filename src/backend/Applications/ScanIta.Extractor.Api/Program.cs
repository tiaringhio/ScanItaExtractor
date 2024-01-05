using QuestPDF.Infrastructure;
using ScanIta.Crawler.Api.Extensions;
using ScanIta.Crawler.Api.Options;
using Serilog;
using ValidateOrThrow;

Log.Logger = WebApplicationBuilderExtensions.CreateBootstrapLogger();

try
{
    Log.Information("Starting API");
    var builder = WebApplication.CreateBuilder(args);

    builder.AddSerilog(builder.Configuration);

    builder.Services.AddApplicationInsightsTelemetry();
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();

    builder.Services.HttpClients(builder.Configuration);

    builder.Services.AddBusiness();

    builder.Services.AddMemoryCache();

    builder.Services.AddOptionsOrThrow<LinkPreviewOptions>();

    QuestPDF.Settings.License = LicenseType.Community;

    var app = builder.Build();

    // expose content-disposition header to allow client to use the pdf filename
    app.UseCors(
        options => options
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition"));

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed {Ex1} {Ex2}", ex, ex.Message);
}
finally
{
    Log.CloseAndFlush();
}
