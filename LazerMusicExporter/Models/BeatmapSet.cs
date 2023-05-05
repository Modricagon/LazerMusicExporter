

using Realms;

namespace LazerMusicExporter.Models;

public partial class BeatmapSet : IRealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }
    public IList<Beatmap> Beatmaps { get; }
    public IList<RealmNamedFileUsage> Files { get; }
}