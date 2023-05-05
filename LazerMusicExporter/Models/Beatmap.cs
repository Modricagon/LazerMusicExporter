using Realms;
// ReSharper disable InconsistentNaming
// Models names must match that of the Osu! Realm database

namespace LazerMusicExporter.Models;

public partial class Beatmap : IRealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }
    public double BPM { get; set; }
    public string? DifficultyName { get; set; }
    public string? MD5Hash { get; set; }
    public string? OnlineMD5Hash { get; set; }
    public BeatmapSet? BeatmapSet { get; set; }
    public BeatmapMetadata? Metadata { get; set; }
}