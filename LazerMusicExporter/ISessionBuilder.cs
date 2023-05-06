using LazerMusicExporter.Core;
using LazerMusicExporter.Models;

namespace LazerMusicExporter;

public interface ISessionBuilder
{
    public IEnumerable<OperationResult<Transaction>> Build(BeatmapSet beatmapSet);
    public OperationResult<Transaction> Build(Beatmap beatmap, string? collectionName);
}