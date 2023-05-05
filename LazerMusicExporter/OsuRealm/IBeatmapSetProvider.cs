using LazerMusicExporter.Models;

namespace LazerMusicExporter.OsuRealm;

public interface IBeatmapSetProvider
{
    public IQueryable<Beatmap> GetCollectionBeatmaps(string collectionName);

    public IQueryable<BeatmapSet> GetAllBeatmapSets();
}