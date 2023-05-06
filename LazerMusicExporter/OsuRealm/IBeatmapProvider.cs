using LazerMusicExporter.Models;

namespace LazerMusicExporter.OsuRealm;

public interface IBeatmapProvider
{
    public IQueryable<Beatmap> GetCollectionBeatmaps(string collectionName);

    public IQueryable<BeatmapSet> GetAllBeatmapSets();
}