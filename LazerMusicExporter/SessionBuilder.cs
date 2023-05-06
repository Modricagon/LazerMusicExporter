using LazerMusicExporter.Configuration;
using LazerMusicExporter.Core;
using LazerMusicExporter.IO;
using LazerMusicExporter.Models;
using LazerMusicExporter.OsuRealm;

namespace LazerMusicExporter;

public class SessionBuilder : ISessionBuilder
{
    private readonly IExportSettings _exportSettings;
    private readonly IOsuFiles _osuFiles;
    private readonly IMetadataStringProvider _metadataStringProvider;

    public SessionBuilder(IExportSettings exportSettings, IOsuFiles osuFiles, IMetadataStringProvider metadataStringProvider)
    {
        _exportSettings = exportSettings;
        _osuFiles = osuFiles;
        _metadataStringProvider = metadataStringProvider;
    }

    public IEnumerable<OperationResult<Transaction>> Build(BeatmapSet beatmapSet)
    {
        var distinctBeatmaps = beatmapSet.DistinctBeatmaps().ToList();
        if (distinctBeatmaps.Count == 1)
        {
            return new []
            {
                BuildSession(distinctBeatmaps.Single(), null)
            };
        }

        return distinctBeatmaps.Where(beatmap => !string.IsNullOrWhiteSpace(beatmap.DifficultyName))
            .Select(beatmap => BuildSession(beatmap, null, beatmap.DifficultyName));
    }

    public OperationResult<Transaction> Build(Beatmap beatmap, string? collectionName) => BuildSession(beatmap, collectionName);

    private OperationResult<Transaction> BuildSession(Beatmap beatmap, string? collectionName, string? version = null)
    {
        var metadata = beatmap.Metadata;
        if (metadata is null)
        {
            return OperationResult<Transaction>.Failed("Beatmap had no metadata");
        }

        var audioFile = beatmap.AudioFile();
        if (audioFile is null)
        {
            return OperationResult<Transaction>.Failed("Beatmap audio file was not registered in the database");
        }

        var audioFileExtension = audioFile.Filename?.Split('.').LastOrDefault();
        if (string.IsNullOrWhiteSpace(audioFileExtension))
        {
            return OperationResult<Transaction>.Failed("Beatmap audio file did not have valid extension");
        }

        var audioHash = audioFile.File?.Hash;
        if (audioHash is null)
        {
            return OperationResult<Transaction>.Failed("Beatmap audio file was not registered in the database");
        }

        var findHashedFileResult = _osuFiles.FindByHash(audioHash);
        if (findHashedFileResult.ResultData is null)
        {
            return OperationResult<Transaction>.Failed("Beatmap audio file did not exist in Osu! files");
        }

        var hashedFile = findHashedFileResult.ResultData;

        // TODO should this fail if has no title or should it replace the title with the song id for example?
        var title = FileHelpers.ReplaceInvalidChars(_metadataStringProvider.GetTitle(metadata));
        if (string.IsNullOrWhiteSpace(title))
        {
            return OperationResult<Transaction>.Failed("Beatmap metadata missing song title");
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
            return OperationResult<Transaction>.Failed("Directory path could not be created");
        }

        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        return OperationResult<Transaction>.Success(new Transaction(beatmap, hashedFile.FullName, outputDirectory,
            fileNameWithExtension));
    }
}