using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace LazerMusicExporter.Configuration;

public class ExportSettings : IExportSettings
{
    private const string OsuDirectoryKey = "osuDirectory";
    private const string OutputDirectoryKey = "outputDirectory";
    private const string IncludeArtworkKey = "includeArtwork";
    private const string CustomArtworkKey = "customArtwork";
    private const string OverwriteModeKey = "overwriteMode";
    private const string NamingSchemeKey = "namingScheme";
    private const string TrackLengthSimilarityKey = "trackLengthSimilarity";
    private const string PreferHigherBitrateKey = "preferHigherBitrate";
    private const string MinimumSongLengthKey = "minimumSongLength";
    private const string CollectionsKey = "collections";

    public ExportSettings(IConfiguration configuration)
    {
        OsuDirectory = configuration.GetValue<string>(OsuDirectoryKey) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(OsuDirectory) || !Directory.Exists(OsuDirectory))
        {
            ReportConfigItemError(OsuDirectoryKey, OsuDirectory);
        }
        else
        {
            ReportConfigItem(OsuDirectoryKey, OsuDirectory);
        }

        OutputDirectory = configuration.GetValue<string>(OutputDirectoryKey) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(OutputDirectory))
        {
            ReportConfigItemError(OutputDirectoryKey, OutputDirectory);
        }
        else
        {
            ReportConfigItem(OutputDirectoryKey, OutputDirectory);
        }

        IncludeArtwork = configuration.GetValue(IncludeArtworkKey, true);
        ReportConfigItem(IncludeArtworkKey, IncludeArtwork.ToString());

        CustomArtworkDirectory = configuration.GetValue<string>(CustomArtworkKey);
        if (!string.IsNullOrWhiteSpace(CustomArtworkDirectory) && !File.Exists(CustomArtworkDirectory))
        {
            ReportConfigItemWarning(CustomArtworkKey, CustomArtworkDirectory + " FILE COULD NOT BE FOUND! IGNORING");
            CustomArtworkDirectory = null;
        }
        else
        {
            ReportConfigItem(CustomArtworkKey, CustomArtworkDirectory);
        }

        var overwriteModeSource = configuration[OverwriteModeKey];
        if (int.TryParse(overwriteModeSource, out var overwriteMode))
        {
            OverwriteMode = (OverwriteMode) overwriteMode;
            ReportConfigItem(OverwriteModeKey, OverwriteMode.ToString());
        }
        else
        {
            ReportConfigItemError(OverwriteModeKey, overwriteModeSource);
        }

        if (Enum.TryParse(typeof(NamingScheme), configuration[NamingSchemeKey], true, out var namingScheme))
        {
            NamingScheme = (NamingScheme) namingScheme;
            ReportConfigItem(NamingSchemeKey, NamingScheme.ToString());
        }
        else
        {
            NamingScheme = NamingScheme.English;
            ReportConfigItemWarning(OverwriteModeKey, NamingScheme + " Default value. ENTRY COULD NOT BE PARSED");
        }

        if (double.TryParse(configuration[TrackLengthSimilarityKey], out var trackLengthSimilarity))
        {
            TrackLengthSimilarity = trackLengthSimilarity;
            ReportConfigItem(TrackLengthSimilarityKey, TrackLengthSimilarity.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            TrackLengthSimilarity = 10;
            ReportConfigItemWarning(OverwriteModeKey, NamingScheme + " Default value. ENTRY COULD NOT BE PARSED");
        }

        if (double.TryParse(configuration[MinimumSongLengthKey], out var minimumSongLength))
        {
            MinimumSongLength = minimumSongLength;
            ReportConfigItem(MinimumSongLengthKey, MinimumSongLength.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            MinimumSongLength = 0;
            ReportConfigItemWarning(MinimumSongLengthKey, MinimumSongLength + " Default value. ENTRY COULD NOT BE PARSED");
        }

        if (bool.TryParse(configuration[PreferHigherBitrateKey], out var preferHigherBitrate))
        {
            PreferHigherBitrate = preferHigherBitrate;
            ReportConfigItem(PreferHigherBitrateKey, PreferHigherBitrate.ToString());
        }
        else
        {
            PreferHigherBitrate = true;
            ReportConfigItem(PreferHigherBitrateKey, PreferHigherBitrate + " Default value. ENTRY COULD NOT BE PARSED");
        }

        var collections = configuration.GetSection(CollectionsKey).GetChildren().Select(s => s.Value);
        Collections = collections.Where(s => !string.IsNullOrWhiteSpace(s)).Cast<string>().ToList();

        ReportConfigItem(CollectionsKey, Collections.Any() ? string.Join(", ", Collections) : "ALL BEATMAPS");
    }

    private static void ReportConfigItem(string key, string? value)
    {
        Console.WriteLine($"{key}: {value}");
    }

    private static void ReportConfigItemWarning(string key, string? value)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        ReportConfigItem(key, value);
        Console.ForegroundColor = ConsoleColor.White;
    }

    private void ReportConfigItemError(string key, string? value)
    {
        IsValid = false;
        Console.ForegroundColor = ConsoleColor.Red;
        ReportConfigItem(key, value + " ERROR");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public bool IsValid { get; private set; } = true;
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