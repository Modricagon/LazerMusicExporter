using LazerMusicExporter.Models;
using Microsoft.Extensions.Logging;

namespace LazerMusicExporter.OsuRealm;

public class BeatmapSetProvider : IBeatmapSetProvider
{
    private readonly IOsuRealmFactory _osuRealmFactory;
    private readonly ILogger<BeatmapSetProvider> _logger;

    public BeatmapSetProvider(IOsuRealmFactory osuRealmFactory, ILogger<BeatmapSetProvider> logger)
    {
        _osuRealmFactory = osuRealmFactory;
        _logger = logger;
    }

    public IQueryable<BeatmapSet> GetCollectionBeatmapSets(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            return GetAllBeatmapSets();
        }

        var realmInstance = _osuRealmFactory.GetInstance();

        _logger.LogInformation("Loading beatmaps from collectionName: {CollectionName}", collectionName);

        var osuCollections = realmInstance.All<BeatmapCollection>();

        // ReSharper disable once InconsistentNaming
        var selectedBeatmapMD5s = osuCollections
            .FirstOrDefault(collection => collection.Name == collectionName)
            ?.BeatmapMD5Hashes ?? new List<string?>();

        var beatmaps = realmInstance.All<Beatmap>();

        var selectedBeatmapSets = Enumerable.Empty<BeatmapSet>().AsQueryable();

        foreach (var beatmap in beatmaps)
        {
            if (beatmap.BeatmapSet is null)
            {
                continue;
            }

            if (selectedBeatmapMD5s.Contains(beatmap.MD5Hash) || selectedBeatmapMD5s.Contains(beatmap.OnlineMD5Hash))
            {

                selectedBeatmapSets = selectedBeatmapSets.Append(beatmap.BeatmapSet);
            }
        }

        _logger.LogInformation("Loaded beatmaps");

        return selectedBeatmapSets;
    }

    public IQueryable<BeatmapSet> GetAllBeatmapSets()
    {
        _logger.LogInformation("Loading beatmaps");
        var beatmaps = _osuRealmFactory.GetInstance().All<BeatmapSet>();
        _logger.LogInformation("Loaded beatmaps");
        return beatmaps;
    }
}