using System.Windows.Input;
using FluentResults;

namespace Application.Contracts;

public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken ct  = default);
}

public interface ICommandHandler<in TCommand> 
{
    Task<Result> Handle(TCommand command, CancellationToken ct = default);
}