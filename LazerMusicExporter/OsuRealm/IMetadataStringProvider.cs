using LazerMusicExporter.Models;

namespace LazerMusicExporter.OsuRealm;

public interface IMetadataStringProvider
{
    public string? GetTitle(BeatmapMetadata beatmapMetadata);
    public string? GetArtist(BeatmapMetadata beatmapMetadata);
    public IEnumerable<string> GetArtists(BeatmapMetadata beatmapMetadata);
}