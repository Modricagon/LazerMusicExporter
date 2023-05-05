using LazerMusicExporter.Models;

namespace LazerMusicExporter.OsuRealm;

public interface IBackgroundFileProvider
{
    public RealmNamedFileUsage? GetBackgroundFile(Beatmap beatmap);
}