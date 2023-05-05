using System.Diagnostics;
using System.Runtime.CompilerServices;
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
                ExportCore(_beatmapSetProvider.GetCollectionBeatmapSets(collection), collection);
            }
        }

        _logger.LogInformation("{NumberOfFiles} new files", _newFiles);
        _logger.LogInformation("{NumberOfFiles} ignored", _ignoredFiles);
        _logger.LogInformation("{NumberOfFiles} failed", _failedFiles);
        _logger.LogInformation("Done");
        Console.ReadKey();
        return Task.CompletedTask;
    }

    private void ExportCore(IQueryable<BeatmapSet> beatmapSets, string? collectionName = null)
    {
        double count = 1;
        foreach (var beatmapSet in beatmapSets)
        {
            Console.Title = $"{collectionName} {Math.Round(count/beatmapSets.Count() * 100, 2)}% {count}/{beatmapSets.Count()}";

            var exportResult = ExportBeatmapSet(beatmapSet, collectionName);

            switch (exportResult.Result)
            {
                case ActionResult.NotApplicable:
                    _ignoredFiles++;
                    break;
                case ActionResult.Failed:
                    var beatmapSetFileName = exportResult.ResultData?.FileName ?? beatmapSet.ID.ToString();
                    _logger.LogError("{ID}: {ErrorMessage}", beatmapSetFileName, exportResult.ErrorMessage);
                    _failedFiles++;
                    break;
                case ActionResult.Success:
                    _newFiles++;
                    break;
                default:
                    throw new UnreachableException();
            }

            count++;
        }
    }

    private OperationResult<ExportSession> ExportBeatmapSet(BeatmapSet beatmapSet, string? collectionName)
    {
        var exportSessionResult = _exportSessionBuilder.BuildFromBeatmapSet(beatmapSet, collectionName);

        if (exportSessionResult.ResultData is null || exportSessionResult.Result is not ActionResult.Success)
        {
            return exportSessionResult;
        }

        var exportSession = exportSessionResult.ResultData;

        var fileCopyResult = _fileWriter.Copy(exportSession.SourceAudioPath, exportSession.DestinationAudioPath);

        if (fileCopyResult.ResultData is null || fileCopyResult.Result is not ActionResult.Success)
        {
            return fileCopyResult.RePackage(() => exportSession);
        }

        var writeMetadataResult = _metadataWriter.Write(fileCopyResult.ResultData, beatmapSet, collectionName);

        if (writeMetadataResult.Result is not ActionResult.Success)
        {
            return fileCopyResult.RePackage(_ => exportSession);
        }

        return OperationResult<ExportSession>.Success(exportSession);
    }
}