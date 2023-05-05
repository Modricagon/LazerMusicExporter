using LazerMusicExporter.Configuration;
using LazerMusicExporter.Core;

namespace LazerMusicExporter.IO;

public class OsuFiles : IOsuFiles
{
    private readonly IExportSettings _exportSettings;
    private readonly string _filesDirectoryName = "files";

    public OsuFiles(IExportSettings exportSettings)
    {
        _exportSettings = exportSettings;
    }

    public OperationResult<FileInfo> FindByHash(string hash)
    {
        var path = _exportSettings.OsuDirectory;

        var rootFolder = Directory.GetDirectories(path);

        if (!rootFolder.Any(directory => directory.EndsWith(_filesDirectoryName)))
        {
            return OperationResult<FileInfo>.Failed($"Could not find root {_filesDirectoryName} folder");
        }

        path = Path.Combine(path, _filesDirectoryName);

        var folderPath = GetFolder(path, hash);

        var filePath = Path.Combine(folderPath, hash);

        return File.Exists(filePath) ? OperationResult<FileInfo>.Success(new FileInfo(filePath)) : OperationResult<FileInfo>.Failed($"Could not find file at {filePath}");
    }

    private static string GetFolder(string path, string hash)
    {
        // TODO this might be Windows only
        var folders = Directory.GetDirectories(path).Select(d => d.Split("\\").Last()).ToList();

        var folder = folders.SingleOrDefault(hash.StartsWith);

        if (folder == null)
        {
            return path;
        }

        path = Path.Combine(path, folder);

        // ReSharper disable once TailRecursiveCall
        return GetFolder(path, hash);
    }
}