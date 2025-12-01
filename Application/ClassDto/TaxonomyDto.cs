namespace Application.Products.ClassDTO;

public class TaxonomyDto
{
    public Guid Id { get; init; }
    public string ProductCategory { get; init; } = string.Empty;
    public string Generation { get; init; } = string.Empty;
    public string Kind { get; init; } = string.Empty;
    
}