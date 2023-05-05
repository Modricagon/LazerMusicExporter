using LazerMusicExporter.Core;
using LazerMusicExporter.Models;

namespace LazerMusicExporter;

public interface IExportSessionBuilder
{
    public IEnumerable<OperationResult<ExportSession>> Build(BeatmapSet beatmapSet, string? collectionName);
    public OperationResult<ExportSession> Build(Beatmap beatmap, string? collectionName);
}