using LazerMusicExporter.Core;
using LazerMusicExporter.Models;

namespace LazerMusicExporter;

public interface IBeatmapSetExporter
{
    public ExportStatistics Export(IQueryable<BeatmapSet> beatmapSets);
}