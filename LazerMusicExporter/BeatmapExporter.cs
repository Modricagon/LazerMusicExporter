using System.Diagnostics;
using LazerMusicExporter.Configuration;
using LazerMusicExporter.Core;
using LazerMusicExporter.IO;
using LazerMusicExporter.Models;
using LazerMusicExporter.OsuRealm;
using Microsoft.Extensions.Logging;

namespace LazerMusicExporter;

public class BeatmapExporter : IBeatmapExporter
{
    private readonly ILogger<BeatmapExporter> _logger;
    private readonly IExportSettings _exportSettings;
    private readonly IBeatmapSetProvider _beatmapSetProvider;

    private readonly IExportSessionBuilder _exportSessionBuilder;
    private readonly IFileWriter _fileWriter;
    private readonly IMetadataWriter _metadataWriter;

    public BeatmapExporter(ILogger<BeatmapExporter> logger, IExportSettings exportSettings,
        IBeatmapSetProvider beatmapSetProvider, IExportSessionBuilder exportSessionBuilder, IFileWriter fileWriter,
        IMetadataWriter metadataWriter)
    {
        _logger = logger;
        _exportSettings = exportSettings;
        _beatmapSetProvider = beatmapSetProvider;
        _exportSessionBuilder = exportSessionBuilder;
        _fileWriter = fileWriter;
        _metadataWriter = metadataWriter;
    }

    private long _newFiles;
    private long _ignoredFiles;
    private long _failedFiles;

    public Task Export()
    {
        if (!_exportSettings.IsValid)
        {
            _logger.LogInformation("Please fix outstanding configuration errors before continuing");
            Console.ReadKey();
            return Task.CompletedTask;
        }

        _logger.LogInformation("Press any key to start with above config");
        Console.ReadKey();

        if (!_exportSettings.Collections.Any())
        {
            ExportCore(_beatmapSetProvider.GetAllBeatmapSets());
        }
        else
        {
            foreach (var collection in _exportSettings.Collections)
            {
                ExportCore(_beatmapSetProvider.GetCollectionBeatmaps(collection), collection);
            }
        }

        _logger.LogInformation("{NumberOfFiles} new files", _newFiles);
        _logger.LogInformation("{NumberOfFiles} ignored", _ignoredFiles);
        _logger.LogInformation("{NumberOfFiles} failed", _failedFiles);
        _logger.LogInformation("Done");
        Console.ReadKey();
        return Task.CompletedTask;
    }

    private void ExportCore<T>(IQueryable<T> items, string? collectionName = null)
    {
        double count = 1;
        double total = items.Count();
        foreach (var item in items)
        {
            Console.Title = $"{collectionName} {Math.Round(count / total * 100, 2)}% {count}/{total}";
            switch (item)
            {
                case Beatmap beatmap:
                    var result = ExportBeatmap(beatmap, collectionName);
                    HandleOperationResult(result);
                    break;
                case BeatmapSet beatmapSet:
                    var results = ExportBeatmapSet(beatmapSet, collectionName);
                    results.ForEach(HandleOperationResult);
                    if (results.Count > 1)
                    {
                        // If multiple audio files were found inside a beatmap set
                        // then update the total to reflect the new found files
                        total += results.Count - 1;
                    }
                    break;
            }
            count++;
        }
    }

    private void HandleOperationResult(OperationResult<ExportSession> exportResult)
    {
        var beatmapSetFileName = exportResult.ResultData?.FileName ?? exportResult.ResultData?.Beatmap.ID.ToString();
        switch (exportResult.Result)
        {
            case ActionResult.NotApplicable:
                _ignoredFiles++;
                break;
            case ActionResult.Failed:
                _logger.LogError("{ID}: {ErrorMessage}", beatmapSetFileName, exportResult.ErrorMessage);
                _failedFiles++;
                break;
            case ActionResult.Success:
                _newFiles++;
                break;
            default:
                throw new UnreachableException();
        }
    }

    private List<OperationResult<ExportSession>> ExportBeatmapSet(BeatmapSet beatmapSet, string? collectionName = null)
    {
        var exportSessionResults = _exportSessionBuilder.Build(beatmapSet, collectionName);

        var results = new List<OperationResult<ExportSession>>();
        foreach (var exportSessionResult in exportSessionResults)
        {
            if (exportSessionResult.ResultData is null || exportSessionResult.Result is not ActionResult.Success)
            {
                results.Add(exportSessionResult);
                continue;
            }

            results.Add(ExportFromSession(exportSessionResult.ResultData, collectionName));
        }

        return results;
    }

    private OperationResult<ExportSession> ExportBeatmap(Beatmap beatmap, string? collectionName = null)
    {
        var exportSessionResult = _exportSessionBuilder.Build(beatmap, collectionName);

        if (exportSessionResult.ResultData is null || exportSessionResult.Result is not ActionResult.Success)
        {
            return exportSessionResult;
        }

        return ExportFromSession(exportSessionResult.ResultData, collectionName);
    }

    private OperationResult<ExportSession> ExportFromSession(ExportSession exportSession, string? collectionName)
    {
        var fileCopyResult = _fileWriter.Copy(exportSession.SourceAudioPath, exportSession.DestinationAudioPath);

        if (fileCopyResult.ResultData is null || fileCopyResult.Result is not ActionResult.Success)
        {
            return fileCopyResult.RePackage(() => exportSession);
        }

        var writeMetadataResult = _metadataWriter.Write(fileCopyResult.ResultData, exportSession.Beatmap, collectionName);

        if (writeMetadataResult.Result is not ActionResult.Success)
        {
            return fileCopyResult.RePackage(_ => exportSession);
        }

        return OperationResult<ExportSession>.Success(exportSession);
    }
}