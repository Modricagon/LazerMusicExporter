namespace LazerMusicExporter.Core;

public class OperationResult
{
    public ActionResult Result;
    public string? ErrorMessage;

    public static OperationResult Success()
    {
        return new OperationResult
        {
            Result = ActionResult.Success
        };
    }

    public static OperationResult Failed(string errorMessage)
    {
        return new OperationResult
        {
            Result = ActionResult.Failed,
            ErrorMessage = errorMessage
        };
    }

    public static OperationResult NotApplicable(string errorMessage)
    {
        return new OperationResult
        {
            Result = ActionResult.NotApplicable,
            ErrorMessage = errorMessage
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

    public new static OperationResult<T> Failed(string errorMessage) =>
        new()
        {
            Result = ActionResult.Failed,
            ErrorMessage = errorMessage
        };

    public new static OperationResult<T> NotApplicable(string errorMessage)
    {
        return new OperationResult<T>
        {
            Result = ActionResult.NotApplicable,
            ErrorMessage = errorMessage
        };
    }
}