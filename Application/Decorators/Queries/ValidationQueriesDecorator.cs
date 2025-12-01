using Application.Context;
using Application.Contracts;
using Application.ResultFactory;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Application.Decorators.Queries;

public sealed class ValidationQueriesDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _handler;
    private readonly ILogger<ValidationQueriesDecorator<TQuery, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TQuery>> _validators;
    private readonly IUserContext _userContext;
    

    public ValidationQueriesDecorator(IQueryHandler<TQuery, TResponse> handler, ILogger<ValidationQueriesDecorator<TQuery, TResponse>> logger, IEnumerable<IValidator<TQuery>> validators, IUserContext userContext)
    {
        _handler = handler;
        _logger = logger;
        _validators = validators;
        _userContext = userContext;
    }
    
    public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken ct = default)
    {
        _logger.LogInformation("Validation - Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {query}",
            _userContext.RequestMethod,
            _userContext.RequestPath,
            _userContext.UserIdentifier,
            query);
     
        if (!_validators.Any())
        {
            _logger.LogInformation("No validator found in query. Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {query}",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                query);
            
            var resultTemp = await _handler.Handle(query, ct);
            
            _logger.LogInformation("Successfully empty validator. Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {query}.",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                query);
            
            return resultTemp;
        }
        
        var tasks = _validators.Select(v => v.ValidateAsync(query, ct));
        var resultTasks = await Task.WhenAll(tasks);
        
        var failures = resultTasks.SelectMany(r => 
                r.Errors ?? Enumerable.Empty<ValidationFailure>())
            .Where(f => true)
            .ToList();

        if (failures.Count > 0)
        {
            var errors = failures .Select(fail =>
                    new Error(fail.ErrorMessage ?? "Validation Error.")
                        .WithMetadata("Message", fail.ErrorMessage))
                .ToList();
    
            _logger.LogError("Validation error in query - Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {query}. Validate errors: {errors}",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                query,
                string.Join(", ", errors.Select(e => e.Message) ));

            return ResultFailFactory.Fail("Validation error in query. ", ErrCodeEnum.Validation, errors);
        }
        
        _logger.LogInformation("Validating query - Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {query}. Ok.",
            _userContext.RequestMethod,
            _userContext.RequestPath,
            _userContext.UserIdentifier,
            query);
        
        var result = await _handler.Handle(query, ct);
        
        _logger.LogInformation("Successfully validated query - Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {query}.",
            _userContext.RequestMethod,
            _userContext.RequestPath,
            _userContext.UserIdentifier,
            query);
        
        return result;
    }
}