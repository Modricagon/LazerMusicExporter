namespace LazerMusicExporter.Core;

public class ExportSession
{
    public string SourceAudioPath;
    public string DestinationAudioPath;
    public string FileName;

    public ExportSession(string sourceAudioPath, string destinationAudioPath, string fileName)
    {
        SourceAudioPath = sourceAudioPath;
        DestinationAudioPath = destinationAudioPath;
        FileName = fileName;
    }
}