using Realms;

namespace LazerMusicExporter.Models;

public partial class BeatmapMetadata : IRealmObject
{
    public string? Title { get; set; }
    public string? TitleUnicode { get; set; }
    public string? Artist { get; set; }
    public string? ArtistUnicode { get; set; }
    public string? Source { get; set; }
    public string? Tags { get; set; }
    public string? AudioFile { get; set; }
    public string? BackgroundFile { get; set; }
}