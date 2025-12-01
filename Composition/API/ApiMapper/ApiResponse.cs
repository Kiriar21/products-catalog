using System.Net;

namespace Composition.API.ApiMapper;

public record ApiResponse<T>(
    bool IsSuccess,
    HttpStatusCode StatusCode,
    T? Data,
    IEnumerable<string>? Errors
);

public record ApiResponse(
    bool IsSuccess,
    HttpStatusCode StatusCode,
    IEnumerable<string>? Errors
);

