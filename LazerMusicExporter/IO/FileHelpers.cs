namespace LazerMusicExporter.IO;

public static class FileHelpers
{
    public static string? ReplaceInvalidChars(string? fileName, string replacement = "_")
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return fileName;
        }

        var okFileName = string.Join(replacement, fileName.Split(Path.GetInvalidFileNameChars()));
        return string.Join(replacement, okFileName.Split(Path.GetInvalidPathChars()));
    }
}