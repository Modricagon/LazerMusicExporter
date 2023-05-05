namespace LazerMusicExporter.Configuration;

public interface IExportSettings
{
    public bool IsValid { get; }
    public string OsuDirectory { get; }
    public string OutputDirectory { get; }
    public bool IncludeArtwork { get; }
    public string? CustomArtworkDirectory { get; }
    public OverwriteMode OverwriteMode { get; }
    public NamingScheme NamingScheme { get; }
    public IEnumerable<string> Collections { get; }
    public double TrackLengthSimilarity { get; }
    public double MinimumSongLength { get; }
    public bool PreferHigherBitrate { get; }
}