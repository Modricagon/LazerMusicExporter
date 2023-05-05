using Realms;
// ReSharper disable InconsistentNaming
// Models names must match that of the Osu! Realm database

namespace LazerMusicExporter.Models;

public partial class BeatmapSet : IRealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }
    public IList<Beatmap> Beatmaps { get; } = Enumerable.Empty<Beatmap>().ToList();
    public IList<RealmNamedFileUsage> Files { get; } = Enumerable.Empty<RealmNamedFileUsage>().ToList();
}