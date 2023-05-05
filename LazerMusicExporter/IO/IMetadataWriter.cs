using LazerMusicExporter.Core;
using LazerMusicExporter.Models;

namespace LazerMusicExporter.IO;

public interface IMetadataWriter
{
    public OperationResult Write(string filePath, BeatmapSet beatmapSet, string? collectionName);
}