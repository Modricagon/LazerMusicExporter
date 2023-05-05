using Realms;

namespace LazerMusicExporter.Models;

public partial class RealmNamedFileUsage : IRealmObject
{
    public File? File { get; set; }
    public string? Filename { get; set; }
}