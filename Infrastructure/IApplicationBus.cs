using Application.Contracts;
using FluentResults;

namespace Infrastructure;

public interface IApplicationBus
{
    Task<Result<TResponse>> ExecuteCommandAsync<TCommand, TResponse>(TCommand command, CancellationToken ct =  default)
        where TCommand : ICommand<TResponse>;
    
    Task<Result> ExecuteEmptyCommandAsync<TCommand>(TCommand command, CancellationToken ct =  default);

    Task<Result<TResponse>> ExecuteQueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
}