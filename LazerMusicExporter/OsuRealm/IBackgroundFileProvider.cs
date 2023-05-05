using LazerMusicExporter.Models;

namespace LazerMusicExporter.OsuRealm;

public interface IBackgroundFileProvider
{
    public RealmNamedFileUsage? GetBackgroundFile(BeatmapSet beatmapSet);
}