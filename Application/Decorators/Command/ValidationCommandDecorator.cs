using Application.Context;
using Application.Contracts;
using Application.ResultFactory;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Application.Decorators.Command;

public sealed class ValidationCommandDecorator<TCommand, TResponse> : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> _handler;
    private readonly ILogger<ValidationCommandDecorator<TCommand, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TCommand>>_validators;
    private readonly IUserContext _userContext;


    public ValidationCommandDecorator(ICommandHandler<TCommand, TResponse> handler, ILogger<ValidationCommandDecorator<TCommand,TResponse>> logger, IEnumerable<IValidator<TCommand>> validators,  IUserContext userContext)
    {
        _handler = handler;
        _logger = logger;
        _validators = validators;
        _userContext = userContext;
    }

    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken ct = default)
    {
       
        _logger.LogInformation("Validation - Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {command}",
            _userContext.RequestMethod,
            _userContext.RequestPath,
            _userContext.UserIdentifier,
            command);

        if (!_validators.Any())
        {
            _logger.LogInformation("No validator found in command. Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {command}", 
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                command);
            
            var resultTemp = await _handler.Handle(command, ct);
            
            _logger.LogInformation("Successfully validated command with no validator. Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {command}",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                command);
            
            return  resultTemp;
        }
        
        var tasks = _validators.Select(v => v.ValidateAsync(command, ct));
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
    
            _logger.LogError("Validation error in command - Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {command}. Validate errors: {errors}",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                command,
                string.Join(", ", errors.Select(e => e.Message) ));

            return ResultFailFactory.Fail("Validation error in command. ", ErrCodeEnum.Validation, errors);
        }

        
        _logger.LogInformation("Validating command - Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {command}. Ok.",
            _userContext.RequestMethod,
            _userContext.RequestPath,
            _userContext.UserIdentifier,
            command);
        
        var result =  await _handler.Handle(command, ct);
        
        _logger.LogInformation("Successfully validated command - Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {command}.",
            _userContext.RequestMethod,
            _userContext.RequestPath,
            _userContext.UserIdentifier,
            command);
        
        return result;
    }

}