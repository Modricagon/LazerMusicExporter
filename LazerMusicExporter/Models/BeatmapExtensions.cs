namespace LazerMusicExporter.Models;

public static class BeatmapExtensions
{
    public static RealmNamedFileUsage? AudioFile(this Beatmap beatmap)
    {
        if (beatmap.BeatmapSet is null)
        {
            return null;
        }

        var audioFileRealName = beatmap.Metadata?.AudioFile;
        if (string.IsNullOrWhiteSpace(audioFileRealName))
        {
            return null;
        }

        var file = beatmap.BeatmapSet.Files.FirstOrDefault(f =>
            audioFileRealName.Equals(f.Filename, StringComparison.InvariantCultureIgnoreCase));

        return string.IsNullOrWhiteSpace(file?.File?.Hash) ? null : file;
    }
}