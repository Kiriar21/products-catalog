namespace Domain.Interfaces;

public interface IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; }
    public string? CreatedBy { get; }
    
    public string? ModifiedBy { get; }
    public DateTimeOffset? ModifiedAt { get; }
}