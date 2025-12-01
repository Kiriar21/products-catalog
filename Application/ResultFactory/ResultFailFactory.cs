using FluentResults;

namespace Application.ResultFactory;

public static class ResultFailFactory
{
    //failes
    public static Result Fail(string message, ErrCodeEnum code, IEnumerable<IError>? causedBy = null)
    {
        var error = new Error(message).WithStatusCode(code);
        if (causedBy != null) error.Reasons.AddRange(causedBy);
        return Result.Fail(error);
    }

    public static Result<T> Fail<T>(string message, ErrCodeEnum code, IEnumerable<IError>? causedBy = null)
    {
        var error = new Error(message).WithStatusCode(code);
        if (causedBy != null) error.Reasons.AddRange(causedBy);
        return Result.Fail<T>(error);
    }

}