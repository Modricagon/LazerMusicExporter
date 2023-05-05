using LazerMusicExporter.Models;

namespace LazerMusicExporter.Core;

public class ExportSession
{
    public Beatmap Beatmap;
    public string SourceAudioPath;
    public string DestinationAudioPath;
    public string FileName;

    public ExportSession(Beatmap beatmap, string sourceAudioPath, string destinationAudioPath, string fileName)
    {
        Beatmap = beatmap;
        SourceAudioPath = sourceAudioPath;
        DestinationAudioPath = destinationAudioPath;
        FileName = fileName;
    }
}