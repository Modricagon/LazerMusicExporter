using LazerMusicExporter.Configuration;
using LazerMusicExporter.Core;

namespace LazerMusicExporter.IO;

public class WindowsFileWriter : IFileWriter
{
    private readonly IExportSettings _exportSettings;

    public WindowsFileWriter(IExportSettings exportSettings)
    {
        _exportSettings = exportSettings;
    }

    public OperationResult<string> Copy(string source, string destination)
    {
        if (_exportSettings.MinimumSongLength > 0)
        {
            var fileExtension = destination.Split('.').Last();
            var sourceFile = TagLib.File.Create(new ReadAsFile(source, fileExtension));
            var songLength = sourceFile.Properties.Duration.TotalSeconds;
            if (songLength < _exportSettings.MinimumSongLength)
            {
                return OperationResult<string>.NotApplicable("Song length was shorter than the specified minimum length");
            }
        }
        
        if (!File.Exists(destination))
        {
            return CopyFile(source, destination);
        }

        return _exportSettings.OverwriteMode switch
        {
            OverwriteMode.None => OverwriteModeNode(source, destination),
            OverwriteMode.IgnoreIfSame => OverwriteModeIgnoreIfSame(source, destination),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Keep all versions of files and index them accordingly
    /// </summary>
    /// <param name="source">Source file path</param>
    /// <param name="destination">Original destination path</param>
    private OperationResult<string> OverwriteModeNode(string source, string destination)
    {
        var existingFile = new FileInfo(destination);
        var fileName = existingFile.Name.Split('.').First();
        var newDestination = destination;

        var index = 0;
        while (File.Exists(newDestination))
        {
            newDestination = $"{fileName} ({index}).{existingFile.Extension}";
            index++;
        }

        return CopyFile(source, newDestination);
    }

    /// <summary>
    /// Ignore files if they are within configured parameters
    /// </summary>
    /// <param name="source">Source file path</param>
    /// <param name="destination">Original destination path</param>
    private OperationResult<string> OverwriteModeIgnoreIfSame(string source, string destination)
    {
        var existingFileInfo = new FileInfo(destination);
        var fileName = existingFileInfo.Name.Split('.').First();
        var sourceFile = TagLib.File.Create(new ReadAsFile(source, existingFileInfo.Extension));

        var newDestination = destination;

        var index = 0;
        while (File.Exists(newDestination))
        {
            var existingFile = TagLib.File.Create(newDestination);

            var lengthDifference = sourceFile.Properties.Duration - existingFile.Properties.Duration;
            if (Math.Abs(lengthDifference.TotalSeconds) < _exportSettings.TrackLengthSimilarity)
            {
                // Track length difference is similar enough

                if (!_exportSettings.PreferHigherBitrate)
                {
                    return OperationResult<string>.NotApplicable("Existing file with similar length already exists");
                }
                    
                if (sourceFile.Properties.AudioBitrate > existingFile.Properties.AudioBitrate)
                {
                    // Overwrite if can get higher bit rate
                    return CopyFile(source, destination);
                }
                return OperationResult<string>.NotApplicable("Existing file had same or better bit rate");
            }

            newDestination = $"{fileName} ({index}).{existingFileInfo.Extension}";
            index++;
        }

        return CopyFile(source, newDestination);
    }

    private OperationResult<string> CopyFile(string source, string destination)
    {
        File.Copy(source, destination, true);
        return OperationResult<string>.Success(destination);
    }
}