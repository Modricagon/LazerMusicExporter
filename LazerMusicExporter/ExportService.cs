using Microsoft.Extensions.Hosting;

namespace LazerMusicExporter;

public class ExportService : BackgroundService
{
    private readonly IBeatmapExporter _beatmapExporter;

    public ExportService(IBeatmapExporter beatmapExporter)
    {
        _beatmapExporter = beatmapExporter;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _beatmapExporter.Export();
    }
}