namespace LazerMusicExporter.Core;

public class OperationResult
{
    public ActionResult Result;
    public string? Message;

    public static OperationResult Success()
    {
        return new OperationResult
        {
            Result = ActionResult.Success
        };
    }

    public static OperationResult Failed(string message)
    {
        return new OperationResult
        {
            Result = ActionResult.Failed,
            Message = message
        };
    }

    public static OperationResult None(string? message = null)
    {
        return new OperationResult
        {
            Result = ActionResult.None,
            Message = message
        };
    }
}

public class OperationResult<T> : OperationResult
{
    public T? ResultData;

    public static OperationResult<T> Success(T resultData) =>
        new()
        {
            Result = ActionResult.Success,
            ResultData = resultData
        };

    public new static OperationResult<T> Failed(string message) =>
        new()
        {
            Result = ActionResult.Failed,
            Message = message
        };

    public new static OperationResult<T> None(string? message = null)
    {
        return new OperationResult<T>
        {
            Result = ActionResult.None,
            Message = message
        };
    }
}