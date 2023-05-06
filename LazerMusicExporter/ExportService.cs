using System.Runtime.CompilerServices;
using LazerMusicExporter.Configuration;
using LazerMusicExporter.Core;
using LazerMusicExporter.OsuRealm;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LazerMusicExporter;

public class ExportService : IHostedService
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


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _hostApplicationLifetime.ApplicationStarted.Register(OnApplicationStarted, false);
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void OnApplicationStarted()
    {
        Run();
        Console.ReadKey();
        _hostApplicationLifetime.StopApplication();
    }

    private void Run()
    {
        if (!_exportSettings.IsValid)
        {
            _logger.LogCritical("Please fix outstanding configuration errors before continuing");
            return;
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
    }
}