using Application.Context;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Database;

internal class AuditabilityEntityInterceptor(IUserContext userContext) : SaveChangesInterceptor
{
    private readonly IUserContext _userContext = userContext;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken ct = new CancellationToken())
    {
        if (eventData.Context != null)
        {
            UpdateAuditableEntities(eventData.Context);
        }
        
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void UpdateAuditableEntities(DbContext context)
    {
        var entities = context
            .ChangeTracker
            .Entries<IAuditableEntity>()
            .ToList();

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entity in entities)
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = utcNow;
                    entity.Property(nameof(IAuditableEntity.CreatedBy)).CurrentValue = _userContext.UserIdentifier;
                    break;
                case EntityState.Modified:
                    entity.Property(nameof(IAuditableEntity.ModifiedAt)).CurrentValue = utcNow;
                    entity.Property(nameof(IAuditableEntity.ModifiedBy)).CurrentValue = _userContext.UserIdentifier;
                    entity.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
                    entity.Property(nameof(IAuditableEntity.CreatedBy)).IsModified = false;
                    break;
                case EntityState.Deleted:
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    break;
            }
        }
    }
}