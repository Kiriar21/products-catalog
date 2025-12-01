using Application.Products.ClassDto;

namespace Application.Products.ClassDTO;

public class ProductSpecificationDto
{
    public Guid Id { get; init; }
    public decimal Price { get; init; }
    public int Amount { get; init; }
    public decimal ValueOfProduct { get; init; }
    public required TaxonomyDto Taxonomy { get; init; } 
    public List<ResourceDto> Resources { get; init; } = new List<ResourceDto>();
    
}