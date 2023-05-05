using LazerMusicExporter.Core;
using LazerMusicExporter.IO;
using LazerMusicExporter.Models;

namespace LazerMusicExporter.OsuRealm;

public class BackgroundFileProvider : IBackgroundFileProvider
{
    private readonly IOsuFiles _osuFiles;

    private readonly string[] _acceptedBackgroundExtensions = { "jpg", "jpeg", "png", "bmp" };

    public BackgroundFileProvider(IOsuFiles osuFiles)
    {
        _osuFiles = osuFiles;
    }

    public RealmNamedFileUsage? GetBackgroundFile(BeatmapSet beatmapSet)
    {
        var backgroundFileRealName = beatmapSet.Metadata()?.BackgroundFile;

        if (string.IsNullOrWhiteSpace(backgroundFileRealName))
        {
            return FallbackBackgroundFile(beatmapSet);
        }

        var file = beatmapSet.Files.FirstOrDefault(f =>
            backgroundFileRealName.Equals(f.Filename, StringComparison.InvariantCultureIgnoreCase));

        if (string.IsNullOrWhiteSpace(file?.File?.Hash))
        {
            return FallbackBackgroundFile(beatmapSet);
        }

        return file;
    }

    /// <summary>
    /// Fallback provider which picks the biggest image file from the beatmap set
    /// </summary>
    /// <param name="beatmapSet"></param>
    /// <returns></returns>
    private RealmNamedFileUsage? FallbackBackgroundFile(BeatmapSet beatmapSet)
    {
        RealmNamedFileUsage? biggestFile = null;
        FileInfo? biggestFileInfo = null;

        foreach (var file in beatmapSet.Files)
        {
            try
            {
                if (file.File is null || string.IsNullOrWhiteSpace(file.File.Hash))
                {
                    continue;
                }

                var fileExtension = file.Filename?.Split('.').LastOrDefault();
                if (string.IsNullOrWhiteSpace(fileExtension) || !_acceptedBackgroundExtensions.Contains(fileExtension))
                {
                    continue;
                }

                var imageFileInfoResult = _osuFiles.FindByHash(file.File.Hash);
                if (imageFileInfoResult.ResultData is null || imageFileInfoResult.Result is not ActionResult.Success)
                {
                    continue;
                }

                var imageFileInfo = imageFileInfoResult.ResultData;

                if (biggestFile is not null && biggestFileInfo is not null &&
                    imageFileInfo.Length <= biggestFileInfo.Length)
                {
                    continue;
                }

                biggestFile = file;
                biggestFileInfo = imageFileInfo;
            }
            catch
            {
                // ignored
            }
        }

        return biggestFile;
    }
}