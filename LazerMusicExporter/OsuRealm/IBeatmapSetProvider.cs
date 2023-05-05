using LazerMusicExporter.Models;

namespace LazerMusicExporter.OsuRealm;

public interface IBeatmapSetProvider
{
    public IQueryable<BeatmapSet> GetCollectionBeatmapSets(string collectionName);

    public IQueryable<BeatmapSet> GetAllBeatmapSets();
}