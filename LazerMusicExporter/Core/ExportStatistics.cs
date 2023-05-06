namespace LazerMusicExporter.Core;

public class ExportStatistics
{
    public double Total { get; set; }
    private double _progress;

    private readonly Dictionary<string, long> _statistics;
    private readonly Func<double, string> _progressTemplate;

    public ExportStatistics(Func<double, string>? progressTemplate = null)
    {
        _statistics = new Dictionary<string, long>();
        _progressTemplate = progressTemplate ?? (completion => $"{completion}%");
    }

    public void AddResult(string result)
    {
        _statistics.TryAdd(result, 0);
        _statistics[result]++;
        _progress++;

        var completion = Math.Round(_progress / Total * 100, 2);
        Console.Title = _progressTemplate(completion);
    }

    public IReadOnlyDictionary<string, long> Results() => _statistics;

    public static ExportStatistics operator +(ExportStatistics source, ExportStatistics additional)
    {
        foreach (var key in additional._statistics.Keys)
        {
            if (!source._statistics.TryAdd(key, additional._statistics[key]))
            {
                source._statistics[key] += additional._statistics[key];
            }
        }

        return source;
    }
}