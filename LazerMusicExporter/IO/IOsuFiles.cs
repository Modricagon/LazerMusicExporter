using LazerMusicExporter.Core;

namespace LazerMusicExporter.IO;

public interface IOsuFiles
{
    public OperationResult<FileInfo> FindByHash(string hash);
}