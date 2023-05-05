using LazerMusicExporter.Core;

namespace LazerMusicExporter.IO;

public interface IFileWriter
{
    public OperationResult<string> Copy(string source, string destination);
}