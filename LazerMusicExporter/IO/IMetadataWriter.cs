using LazerMusicExporter.Core;
using LazerMusicExporter.Models;

namespace LazerMusicExporter.IO;

public interface IMetadataWriter
{
    public OperationResult Write(string filePath, Beatmap beatmap, string? collectionName);
}