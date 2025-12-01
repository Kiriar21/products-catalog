using Application.ClassDto;
using FluentResults;

namespace Application.Contracts;

public interface IQueryHandler<in TQuery, TResponse> where TQuery: IQuery<TResponse> 
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken ct = default);
}