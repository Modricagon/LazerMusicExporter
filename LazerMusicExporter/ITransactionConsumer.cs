using LazerMusicExporter.Core;

namespace LazerMusicExporter
{
    public interface ITransactionConsumer
    {
        public OperationResult Consume(Transaction transaction, string? collectionName = null);
    }
}
