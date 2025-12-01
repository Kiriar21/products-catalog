using Application.Context;
using Application.Contracts;
using Application.ResultFactory;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Decorators.EmptyCommand;

public class TryCatchEmptyCommandDecorator<TCommand> : ICommandHandler<TCommand>
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly ILogger<TryCatchEmptyCommandDecorator<TCommand>> _logger;
    private readonly IUserContext _userContext;

    public TryCatchEmptyCommandDecorator(ICommandHandler<TCommand> handler, ILogger<TryCatchEmptyCommandDecorator<TCommand>> logger,  IUserContext userContext)
    {
        _handler = handler;
        _logger = logger;
        _userContext = userContext;
        
    }
    public async Task<Result> Handle(TCommand command, CancellationToken ct = default)
    {
        try
        {
            
            _logger.LogInformation("TryCatch - Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {command}",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                command);
            
            var result = await _handler.Handle(command, ct);
            
            _logger.LogInformation("Executed successfully command. Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {command}",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                command);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to handle the command: Method: {Method}. Path: {Path} UserId: {userInfo}. Req: {command}. Exception: {ex}",
                _userContext.RequestMethod,
                _userContext.RequestPath,
                _userContext.UserIdentifier,
                command,
                ex.Message);
            
            var err = new ExceptionalError(ex);
            return ResultFailFactory.Fail("Unexcepted error.",ErrCodeEnum.Unexpected, causedBy: [err]);

        }
    }

}