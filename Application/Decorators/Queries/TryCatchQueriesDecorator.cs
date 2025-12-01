using Application.Context;
using Application.Contracts;
using Application.ResultFactory;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Decorators.Queries;

public sealed class TryCatchQueriesDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _handler;
    private readonly ILogger<TryCatchQueriesDecorator<TQuery, TResponse>> _logger;
    private readonly IUserContext _userContext;

    public TryCatchQueriesDecorator(IQueryHandler<TQuery, TResponse> handler, ILogger<TryCatchQueriesDecorator<TQuery, TResponse>> logger, IUserContext userContext)
    {
        _handler = handler;
        _logger = logger;
        _userContext = userContext;
    }
    
    public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken ct = default)
    {
        try
        {
            
            _logger.LogInformation("TryCatch - Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {query}",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                query);
            
            var result =  await _handler.Handle(query, ct);
            _logger.LogInformation("Executed successfully query. Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {query}",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                query);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to handle the query. Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {query}. Exception: {ex}",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                query,
                ex.Message);
            
            var err = new ExceptionalError(ex);
            return ResultFailFactory.Fail("Unexcepted error.", ErrCodeEnum.Unexpected, causedBy: [err]);
        }
    }
}