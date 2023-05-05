namespace LazerMusicExporter.Core;

public static class OperationResultExtensions
{
    public static OperationResult<TOut> RePackage<TOut>(this OperationResult result, Func<TOut> property)
    {
        return new OperationResult<TOut>
        {
            ErrorMessage = result.ErrorMessage,
            Result = result.Result,
            ResultData = property()
        };
    }

    public static OperationResult<TOut> RePackage<TIn, TOut>(this OperationResult<TIn> result,
        Func<TIn?, TOut> property)
    {
        return new OperationResult<TOut>
        {
            ErrorMessage = result.ErrorMessage,
            Result = result.Result,
            ResultData = property(result.ResultData)
        };
    }
}