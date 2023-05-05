namespace LazerMusicExporter.Models;

public static class BeatmapSetExtensions
{
    public static IEnumerable<Beatmap> DistinctBeatmaps(this BeatmapSet beatmapSet)
    {
        var beatmaps = beatmapSet.Beatmaps;
        return beatmaps.Where(beatmap => beatmap.Metadata is not null)
            .DistinctBy(beatmap => beatmap.Metadata!.AudioFile);
    }
}