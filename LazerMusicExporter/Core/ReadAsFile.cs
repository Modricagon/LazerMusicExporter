namespace LazerMusicExporter.Core;

public class ReadAsFile : TagLib.File.IFileAbstraction
{
    private readonly string _path;
    private readonly string _extension;

    public ReadAsFile(string path, string extension)
    {
        _path = path;
        _extension = extension;
    }

    public void CloseStream(Stream stream)
    {
        stream.Close();
    }

    public string Name => $"{_path}.{_extension}";

    public Stream ReadStream => File.OpenRead(_path);

    public Stream WriteStream => throw new NotImplementedException("We do not want to write to these files");
}