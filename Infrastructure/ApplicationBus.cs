using Application.Contracts;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

internal class ApplicationBus(IServiceProvider root) : IApplicationBus
{

    public async Task<Result> ExecuteEmptyCommandAsync<TCommand>(TCommand command, CancellationToken ct = default)
    {
        var commandType = command!.GetType();
        var handlerInterface = typeof(ICommandHandler<>).MakeGenericType(commandType);
        var handler = root.GetRequiredService(handlerInterface);
        var method = handlerInterface.GetMethod("Handle")!;
        var task = (Task<Result>)method.Invoke(handler, [command, ct])!;
        return await task.ConfigureAwait(false);
    }


    public async Task<Result<TResponse>> ExecuteCommandAsync<TCommand, TResponse>(TCommand command,
        CancellationToken ct = default) where TCommand : ICommand<TResponse>
    {
        var commandType = command.GetType();
        var handlerInterface = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));
        var handler = root.GetRequiredService(handlerInterface);
        var method = handlerInterface.GetMethod("Handle")!;

        var task = (Task<Result<TResponse>>)method.Invoke(handler, [command, ct])!;
        return await task.ConfigureAwait(false);
    }

    
    public async Task<Result<TResponse>> ExecuteQueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct = default)
    {
        var queryType = query.GetType();
        var handlerInterface = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
        var handler = root.GetRequiredService(handlerInterface);
        var method = handlerInterface.GetMethod("Handle")!; 

        var task = (Task<Result<TResponse>>)method.Invoke(handler, [query, ct])!;
        return await task.ConfigureAwait(false);
    }
}