using Realms;

namespace LazerMusicExporter.Models;

public partial class BeatmapCollection : IRealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }
    public string? Name { get; set; }
    public IList<string?> BeatmapMD5Hashes { get; }
}