using LazerMusicExporter.Core;
using LazerMusicExporter.Models;

namespace LazerMusicExporter;

public interface IBeatmapExporter
{
    public ExportStatistics Export(IQueryable<Beatmap> beatmaps, string? collectionName = null);
}