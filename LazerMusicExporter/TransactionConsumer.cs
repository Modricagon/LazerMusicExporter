using LazerMusicExporter.Core;
using LazerMusicExporter.IO;

namespace LazerMusicExporter;

public class TransactionConsumer : ITransactionConsumer
{
    private readonly IFileWriter _fileWriter;
    private readonly IMetadataWriter _metadataWriter;

    public TransactionConsumer(IFileWriter fileWriter, IMetadataWriter metadataWriter)
    {
        _fileWriter = fileWriter;
        _metadataWriter = metadataWriter;
    }

    public OperationResult Consume(Transaction transaction, string? collectionName = null)
    {
        var fileCopyResult = _fileWriter.Copy(transaction.SourceAudioPath, transaction.DestinationAudioPath);

        if (fileCopyResult.ResultData is null || fileCopyResult.Result is not ActionResult.Success)
        {
            return fileCopyResult;
        }

        var writeMetadataResult = _metadataWriter.Write(fileCopyResult.ResultData, transaction.Beatmap, collectionName);

        if (writeMetadataResult.Result is not ActionResult.Success)
        {
            return fileCopyResult;
        }

        Console.Title = transaction.FileName;
        return OperationResult.Success();
    }
}