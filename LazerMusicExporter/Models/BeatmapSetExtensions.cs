namespace LazerMusicExporter.Models;

public static class BeatmapSetExtensions
{
    public static RealmNamedFileUsage? AudioFile(this BeatmapSet beatmapSet)
    {
        RealmNamedFileUsage? audioFile = null;

        foreach (var beatmap in beatmapSet.Beatmaps)
        {
            if (audioFile is not null)
            {
                return audioFile;
            }

            var audioFileRealName = beatmap.Metadata?.AudioFile;
            if (string.IsNullOrWhiteSpace(audioFileRealName))
            {
                continue;
            }

            var file = beatmapSet.Files.FirstOrDefault(f =>
                audioFileRealName.Equals(f.Filename, StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrWhiteSpace(file?.File?.Hash))
            {
                continue;
            }

            audioFile = file;
        }

        return audioFile;
    }

    public static BeatmapMetadata? Metadata(this BeatmapSet beatmapSet)
    {
        return beatmapSet.Beatmaps.FirstOrDefault()?.Metadata;
    }
}