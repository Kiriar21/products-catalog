namespace Application.Products.ClassDTO;

public class VersionProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int VersionNumber { get; init; }
    public bool IsObsolete { get; init; }
    
    public DateTimeOffset? CreatedAt { get; init; }
    public string? CreatedBy { get; init; } 
    public DateTimeOffset? ModifiedAt { get; init; }
    public string? ModifiedBy { get; init; }

    public ProductSpecificationDto? Specifications { get; init; }
    
}