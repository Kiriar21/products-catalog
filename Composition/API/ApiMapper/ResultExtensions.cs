using System.Net;
using System.Text.Json;
using Application.ResultFactory;
using FluentResults;

namespace Composition.API.ApiMapper;

public static class ResultExtensions
{
    private static IEnumerable<IError> CollectAllErrors(IError error)
    {
        yield return error;

        foreach (var inner in error.Reasons.OfType<IError>())
        {
            foreach (var m in CollectAllErrors(inner))
                yield return m;
        }
    }
    
    public static IResult ToApi<T>(this Result<T> result, HttpStatusCode? statusCode = null)
    {
        if (result.IsSuccess)
        {
            var statusCodeTemp = statusCode ?? HttpStatusCode.OK;
            var response = new ApiResponse<T>(true, statusCodeTemp, result.Value, null);
            return Results.Json(response, statusCode: (int)statusCodeTemp );
        }
        
        var firstWithCode = result.Errors
            .SelectMany(CollectAllErrors)
            .FirstOrDefault(e => e.GetStatusCode() != null);

        var errCode = firstWithCode?.GetStatusCode() ?? ErrCodeEnum.Unexpected;

        var statusErrCode = errCode switch
        {
            ErrCodeEnum.Conflict   => HttpStatusCode.Conflict,
            ErrCodeEnum.Forbidden  => HttpStatusCode.Forbidden,
            ErrCodeEnum.NotFound   => HttpStatusCode.NotFound,
            ErrCodeEnum.Validation => HttpStatusCode.UnprocessableEntity,
            ErrCodeEnum.Unexpected => HttpStatusCode.ServiceUnavailable,
            _ => HttpStatusCode.UnprocessableEntity
        };
        
        var responseError = new ApiResponse<T>(
            false,
            statusErrCode,
            default,
            result.Errors.SelectMany(CollectAllErrors)
                .Select(e => e.Message)
            );

        return Results.Json(responseError, statusCode: (int)statusErrCode );

    }
    
    public static IResult ToApi(this Result result, HttpStatusCode? statusCode = null)
    {
        if (result.IsSuccess)
        {
            var statusCodeTemp = statusCode ?? HttpStatusCode.OK;
            var response = new ApiResponse(true, statusCodeTemp, null);
            return Results.Json(data: response, statusCode: (int)statusCodeTemp );
        }

        var firstError = result.Errors.FirstOrDefault();
        
        var firstWithCode = result.Errors
            .SelectMany(CollectAllErrors)
            .FirstOrDefault(e => e.GetStatusCode() != null);

        var errCode = firstWithCode?.GetStatusCode() ?? ErrCodeEnum.Unexpected;

        var statusErrCode = errCode switch
        {
            ErrCodeEnum.Conflict   => HttpStatusCode.Conflict,
            ErrCodeEnum.Forbidden  => HttpStatusCode.Forbidden,
            ErrCodeEnum.NotFound   => HttpStatusCode.NotFound,
            ErrCodeEnum.Validation => HttpStatusCode.UnprocessableEntity,
            ErrCodeEnum.Unexpected => HttpStatusCode.ServiceUnavailable,
            _ => HttpStatusCode.UnprocessableEntity
        };
        
        var responseError = new ApiResponse(
            false,
            statusErrCode,
            result.Errors.SelectMany(CollectAllErrors)
                .Select(e => e.Message)
        );

        return Results.Json(responseError,  statusCode: (int)statusErrCode );

    }
}