using LazerMusicExporter.Core;
using LazerMusicExporter.Models;

namespace LazerMusicExporter;

public interface IExportSessionBuilder
{
    public OperationResult<ExportSession> BuildFromBeatmapSet(BeatmapSet beatmapSet, string? collectionName);
}