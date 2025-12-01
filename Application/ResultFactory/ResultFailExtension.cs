using FluentResults;

namespace Application.ResultFactory;

public static class ResultFailExtension
{
    public static IError WithStatusCode(this IError error, ErrCodeEnum code)
    {
        error.Metadata["StatusCode"] = code;
        return error;
    }

    public static ErrCodeEnum? GetStatusCode(this IError error)
    {
        if (error.Metadata.TryGetValue("StatusCode", out var value) && value is ErrCodeEnum code)
            return code;

        return null;
    }
}