namespace Application.Contracts;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}