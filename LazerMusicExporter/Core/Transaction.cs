using LazerMusicExporter.Models;

namespace LazerMusicExporter.Core;

public class Transaction
{
    public Beatmap Beatmap;
    public string SourceAudioPath;
    public string DestinationAudioPath;
    public string FileName;

    public Transaction(Beatmap beatmap, string sourceAudioPath, string destinationAudioPath, string fileName)
    {
        Beatmap = beatmap;
        SourceAudioPath = sourceAudioPath;
        DestinationAudioPath = destinationAudioPath;
        FileName = fileName;
    }
}