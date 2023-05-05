using LazerMusicExporter.Configuration;
using LazerMusicExporter.Core;
using LazerMusicExporter.IO;
using LazerMusicExporter.Models;
using LazerMusicExporter.OsuRealm;

namespace LazerMusicExporter;

public class ExportSessionBuilder : IExportSessionBuilder
{
    private readonly IExportSettings _exportSettings;
    private readonly IOsuFiles _osuFiles;
    private readonly IMetadataStringProvider _metadataStringProvider;

    public ExportSessionBuilder(IExportSettings exportSettings, IOsuFiles osuFiles, IMetadataStringProvider metadataStringProvider)
    {
        _exportSettings = exportSettings;
        _osuFiles = osuFiles;
        _metadataStringProvider = metadataStringProvider;
    }

    public IEnumerable<OperationResult<ExportSession>> Build(BeatmapSet beatmapSet, string? collectionName)
    {
        var distinctBeatmaps = beatmapSet.DistinctBeatmaps().ToList();
        if (distinctBeatmaps.Count == 1)
        {
            return new []
            {
                BuildSession(distinctBeatmaps.Single(), collectionName)
            };
        }

        return distinctBeatmaps.Where(beatmap => !string.IsNullOrWhiteSpace(beatmap.DifficultyName))
            .Select(beatmap => BuildSession(beatmap, collectionName, beatmap.DifficultyName));
    }

    public OperationResult<ExportSession> Build(Beatmap beatmap, string? collectionName) => BuildSession(beatmap, collectionName);

    private OperationResult<ExportSession> BuildSession(Beatmap beatmap, string? collectionName, string? version = null)
    {
        var metadata = beatmap.Metadata;
        if (metadata is null)
        {
            return OperationResult<ExportSession>.Failed("Metadata was null");
        }

        var audioFile = beatmap.AudioFile();
        if (audioFile is null)
        {
            return OperationResult<ExportSession>.Failed("Could not find matching audio file");
        }

        var audioFileExtension = audioFile.Filename?.Split('.').LastOrDefault();
        if (string.IsNullOrWhiteSpace(audioFileExtension))
        {
            return OperationResult<ExportSession>.Failed("Audio file did not have valid extension");
        }

        var audioHash = audioFile.File?.Hash;
        if (audioHash is null)
        {
            return OperationResult<ExportSession>.Failed("No audio file hash found");
        }

        var findHashedFileResult = _osuFiles.FindByHash(audioHash);
        if (findHashedFileResult.ResultData is null)
        {
            return OperationResult<ExportSession>.Failed("Could not find file by hash");
        }

        var hashedFile = findHashedFileResult.ResultData;

        var title = FileHelpers.ReplaceInvalidChars(_metadataStringProvider.GetTitle(metadata));
        if (string.IsNullOrWhiteSpace(title))
        {
            return OperationResult<ExportSession>.Failed("Title was null or white space");
        }

        var artist = FileHelpers.ReplaceInvalidChars(_metadataStringProvider.GetArtist(metadata));

        var fileName = string.IsNullOrWhiteSpace(artist) ? title : $"{artist} - {title}";
        var fileNameWithExtension = !string.IsNullOrWhiteSpace(version)
            ? $"{fileName} [{FileHelpers.ReplaceInvalidChars(version)}].{audioFileExtension}"
            : $"{fileName}.{audioFileExtension}";

        var outputDirectory = string.IsNullOrWhiteSpace(collectionName)
            ? Path.Combine(_exportSettings.OutputDirectory, fileNameWithExtension)
            : Path.Combine(_exportSettings.OutputDirectory, FileHelpers.ReplaceInvalidChars(collectionName)!, fileNameWithExtension);

        var directoryName = new FileInfo(outputDirectory).DirectoryName;
        if (string.IsNullOrWhiteSpace(directoryName))
        {
            return OperationResult<ExportSession>.Failed("Directory could not be created");
        }

        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        return OperationResult<ExportSession>.Success(new ExportSession(beatmap, hashedFile.FullName, outputDirectory,
            fileNameWithExtension));
    }
}