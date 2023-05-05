using Realms;

namespace LazerMusicExporter.Models;

public partial class File : IRealmObject
{
    [PrimaryKey]
    public string? Hash { get; set; }
}