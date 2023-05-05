using LazerMusicExporter.Configuration;
using LazerMusicExporter.Models;

namespace LazerMusicExporter.OsuRealm;

public class MetadataStringProvider : IMetadataStringProvider
{
    private readonly IExportSettings _exportSettings;

    public MetadataStringProvider(IExportSettings exportSettings)
    {
        _exportSettings = exportSettings;
    }

    public string? GetTitle(BeatmapMetadata beatmapMetadata)
    {
        return GetNameByScheme(beatmapMetadata.Title, beatmapMetadata.TitleUnicode);
    }

    public string? GetArtist(BeatmapMetadata beatmapMetadata)
    {
        return GetNameByScheme(beatmapMetadata.Artist, beatmapMetadata.ArtistUnicode);
    }

    public IEnumerable<string> GetArtists(BeatmapMetadata beatmapMetadata)
    {
        return (_exportSettings.NamingScheme switch
        {
            NamingScheme.English => new[] { beatmapMetadata.Artist, beatmapMetadata.ArtistUnicode },
            NamingScheme.Native => new[] { beatmapMetadata.ArtistUnicode, beatmapMetadata.Artist },
            _ => throw new ArgumentOutOfRangeException()
        }).Where(s => !string.IsNullOrWhiteSpace(s)).Cast<string>();
    }

    private string? GetNameByScheme(string? english, string? native)
    {
        return _exportSettings.NamingScheme switch
        {
            NamingScheme.English => string.IsNullOrWhiteSpace(english) ? native : english,
            NamingScheme.Native => string.IsNullOrWhiteSpace(native) ? english : native,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}