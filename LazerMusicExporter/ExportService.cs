using LazerMusicExporter.Configuration;
using LazerMusicExporter.Core;
using LazerMusicExporter.OsuRealm;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LazerMusicExporter;

public class ExportService : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<ExportService> _logger;
    private readonly IExportSettings _exportSettings;
    private readonly IBeatmapSetExporter _beatmapSetExporter;
    private readonly IBeatmapExporter _beatmapExporter;
    private readonly IBeatmapProvider _beatmapProvider;

    public ExportService(IHostApplicationLifetime hostApplicationLifetime, ILogger<ExportService> logger,
        IExportSettings exportSettings, IBeatmapSetExporter beatmapSetExporter, IBeatmapExporter beatmapExporter,
        IBeatmapProvider beatmapProvider)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
        _exportSettings = exportSettings;
        _beatmapSetExporter = beatmapSetExporter;
        _beatmapExporter = beatmapExporter;
        _beatmapProvider = beatmapProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_exportSettings.IsValid)
        {
            _logger.LogCritical("Please fix outstanding configuration errors before continuing");
            Console.ReadKey();
            return Task.CompletedTask;
        }

        _logger.LogInformation("Press any key to start with above config");
        Console.ReadKey();

        var statistics = new ExportStatistics();
        if (!_exportSettings.Collections.Any())
        {
            statistics = _beatmapSetExporter.Export(_beatmapProvider.GetAllBeatmapSets());
        }
        else
        {
            foreach (var collection in _exportSettings.Collections)
            {
                statistics += _beatmapExporter.Export(_beatmapProvider.GetCollectionBeatmaps(collection), collection);
            }
        }

        foreach (var (key, value) in statistics.Results())
        {
            _logger.LogInformation("{Count} {Type}", value, key);
        }
        _logger.LogInformation("Done");

        Console.ReadKey();
        _hostApplicationLifetime.StopApplication();
        return Task.CompletedTask;
    }
}