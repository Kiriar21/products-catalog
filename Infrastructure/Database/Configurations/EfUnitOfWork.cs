using Application.Contracts;

namespace Infrastructure.Database.Configurations;

public class EfUnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
   public Task<int> CommitAsync(CancellationToken ct = default) => 
        dbContext.SaveChangesAsync(ct);
   
}