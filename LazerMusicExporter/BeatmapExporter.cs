using LazerMusicExporter.Core;
using LazerMusicExporter.Models;

namespace LazerMusicExporter;

public class BeatmapExporter : IBeatmapExporter
{
    private readonly ISessionBuilder _sessionBuilder;
    private readonly ITransactionConsumer _transactionConsumer;

    public BeatmapExporter(ISessionBuilder sessionBuilder, ITransactionConsumer transactionConsumer)
    {
        _sessionBuilder = sessionBuilder;
        _transactionConsumer = transactionConsumer;
    }

    public ExportStatistics Export(IQueryable<Beatmap> beatmaps, string? collectionName = null)
    {
        var exportStatistics = new ExportStatistics(completion => $"{collectionName} {completion}%")
        {
            Total = beatmaps.LongCount()
        };

        foreach (var beatmap in beatmaps)
        {
            var exportResult = ExportBeatmap(beatmap, collectionName);
            exportStatistics.AddResult(exportResult.Message ?? exportResult.Result.ToString());
        }

        return exportStatistics;
    }

    private OperationResult ExportBeatmap(Beatmap beatmap, string? collectionName = null)
    {
        var sessionResult = _sessionBuilder.Build(beatmap, collectionName);

        if (sessionResult.ResultData is null || sessionResult.Result is not ActionResult.Success)
        {
            return sessionResult;
        }

        return _transactionConsumer.Consume(sessionResult.ResultData, collectionName);
    }
}