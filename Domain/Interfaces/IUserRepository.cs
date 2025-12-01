namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct);
    Task<User?> GetByProviderAsync(string provider, string providerSubject, CancellationToken ct = default);
}