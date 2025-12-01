using Application.Products.ClassDTO;

namespace Application.ClassDto;

public class ProductDto
{
    public Guid Id { get; init; }
    public string ProductType { get; init; } = string.Empty;
    public string LifeCycleStatus { get; init; } = string.Empty;
    
    public DateTimeOffset? CreatedAt { get; init; }
    public string? CreatedBy { get; init; } 
    public DateTimeOffset? ModifiedAt { get; init; }
    public string? ModifiedBy { get; init; }
    
    public List<VersionProductDto> VersionProducts { get; init; } = new List<VersionProductDto>();
}