using Domain;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EfUsersRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _context.Users.SingleOrDefaultAsync(x => x.Id == id, ct);
    }
    
    public Task<User?> GetByProviderAsync(string provider, string providerSubject, CancellationToken ct = default)
        => _context.Users.SingleOrDefaultAsync(x => x.AuthProvider == provider && x.ProviderSubject == providerSubject, ct);

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
    }
}