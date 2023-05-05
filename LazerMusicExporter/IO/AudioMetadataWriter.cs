using LazerMusicExporter.Configuration;
using LazerMusicExporter.Core;
using LazerMusicExporter.Models;
using LazerMusicExporter.OsuRealm;
using Microsoft.Extensions.Logging;
using TagLib;
using File = System.IO.File;

namespace LazerMusicExporter.IO;

public class AudioMetadataWriter : IMetadataWriter
{
    private readonly IExportSettings _exportSettings;
    private readonly ILogger<AudioMetadataWriter> _logger;
    private readonly IMetadataStringProvider _metadataStringProvider;
    private readonly IBackgroundFileProvider _backgroundFileProvider;
    private readonly IOsuFiles _osuFiles;

    public AudioMetadataWriter(IExportSettings exportSettings, ILogger<AudioMetadataWriter> logger, IMetadataStringProvider metadataStringProvider, IBackgroundFileProvider backgroundFileProvider, IOsuFiles osuFiles)
    {
        _exportSettings = exportSettings;
        _logger = logger;
        _metadataStringProvider = metadataStringProvider;
        _backgroundFileProvider = backgroundFileProvider;
        _osuFiles = osuFiles;
    }

    public OperationResult Write(string filePath, BeatmapSet beatmapSet, string? collectionName)
    {
        var file = TagLib.File.Create(filePath);

        var writeResult = WriteCore(file, beatmapSet, collectionName);

        file.Save();
        file.Dispose();
        return writeResult;
    }

    private OperationResult WriteCore(TagLib.File file, BeatmapSet beatmapSet, string? collectionName)
    {
        var metadata = beatmapSet.Metadata();

        if (metadata is not null)
        {

            file.Tag.Title = _metadataStringProvider.GetTitle(metadata);

            if (!file.Tag.Performers.Any())
            {
                file.Tag.Performers = _metadataStringProvider.GetArtists(metadata).ToArray();
            }

            if (!file.Tag.AlbumArtists.Any())
            {
                file.Tag.AlbumArtists = _metadataStringProvider.GetArtists(metadata).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(collectionName))
            {
                file.Tag.Album = collectionName;
            }

            if (string.IsNullOrWhiteSpace(file.Tag.Album))
            {
                file.Tag.Album = metadata.Source;
            }

            file.Tag.Description = metadata.Tags;

            file.Tag.Comment = "Exported with LazerMusicExporter";
        }

        var bpm = beatmapSet.Beatmaps.FirstOrDefault(beatmap => beatmap.BPM > 0)?.BPM;

        if (bpm is not null)
        {
            var roundedBpm = Math.Round(bpm.Value);
            file.Tag.BeatsPerMinute = Convert.ToUInt32(roundedBpm);
        }

        if (!_exportSettings.IncludeArtwork)
        {
            return OperationResult.Success();
        }

        if (!string.IsNullOrWhiteSpace(_exportSettings.CustomArtworkDirectory) &&
            File.Exists(_exportSettings.CustomArtworkDirectory))
        {
            file.Tag.Pictures = new IPicture[]
            {
                new Picture(_exportSettings.CustomArtworkDirectory)
            };
            return OperationResult.Success();
        }

        var backgroundFile = _backgroundFileProvider.GetBackgroundFile(beatmapSet);
        if (backgroundFile is null || string.IsNullOrWhiteSpace(backgroundFile.File?.Hash))
        {
            return OperationResult.Success();
        }

        var backgroundFileExtension = backgroundFile.Filename?.Split('.').LastOrDefault();

        if (string.IsNullOrWhiteSpace(backgroundFileExtension))
        {
            return OperationResult.Success();
        }

        var hashedFileResult = _osuFiles.FindByHash(backgroundFile.File.Hash);
        if (hashedFileResult.ResultData is null || hashedFileResult.Result is not ActionResult.Success)
        {
            return OperationResult.Success();
        }

        file.Tag.Pictures = new IPicture[]
        {
            new Picture(new ReadAsFile(hashedFileResult.ResultData.FullName, backgroundFileExtension))
        };

        return OperationResult.Success();
    }
}