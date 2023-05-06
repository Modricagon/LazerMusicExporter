using LazerMusicExporter.Core;
using LazerMusicExporter.Models;

namespace LazerMusicExporter;

public class BeatmapSetExporter : IBeatmapSetExporter
{
    private readonly ISessionBuilder _sessionBuilder;
    private readonly ITransactionConsumer _transactionConsumer;

    public BeatmapSetExporter(ISessionBuilder sessionBuilder, ITransactionConsumer transactionConsumer)
    {
        _sessionBuilder = sessionBuilder;
        _transactionConsumer = transactionConsumer;
    }

    public ExportStatistics Export(IQueryable<BeatmapSet> beatmapSets)
    {
        var exportStatistics = new ExportStatistics(ProgressTemplate)
        {
            Total = beatmapSets.Count()
        };

        foreach (var beatmapSet in beatmapSets)
        {
            var exportResults = ExportBeatmapSet(beatmapSet);
            exportResults.ForEach(exportResult => exportStatistics.AddResult(exportResult.Message ?? exportResult.Result.ToString()));

            if (exportResults.Count > 1)
            {
                exportStatistics.Total += exportResults.Count - 1;
            }
        }
        return exportStatistics;
    }

    private List<OperationResult> ExportBeatmapSet(BeatmapSet beatmapSet)
    {
        var transactionResults = _sessionBuilder.Build(beatmapSet);

        var results = new List<OperationResult>();
        foreach (var transactionResult in transactionResults)
        {
            if (transactionResult.ResultData is null || transactionResult.Result is not ActionResult.Success)
            {
                results.Add(transactionResult);
                continue;
            }

            results.Add(_transactionConsumer.Consume(transactionResult.ResultData));
        }

        return results;
    }

    private static string ProgressTemplate(double completion) => $"All Beatmaps {completion}%";
}