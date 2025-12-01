using Application.ClassDto;
using Application.Products.ClassDto;
using Application.Products.ClassDTO;
using Domain;

namespace Application.Mappers;

public static class ProductMapping
{
    //Product -> ProductDTO
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            ProductType = product.GetProductType.GetProductTypeName(),
            LifeCycleStatus = product.GetLifeCycleStatus.ToString(),
            CreatedAt = product.CreatedAt,
            CreatedBy = product.CreatedBy,
            ModifiedAt = product.ModifiedAt,
            ModifiedBy = product.ModifiedBy,
            VersionProducts = product.Versions
                .Select(v => v!.ToDto())
                .Where(v => !v.IsObsolete)
                .OrderBy(v => v.VersionNumber)
                .ToList()
        };
    }
    
    //VersionProduct -> VersionProductDTO
    public static VersionProductDto ToDto(this VersionProduct versionProduct)
    {
        return new VersionProductDto
        {
            Id = versionProduct.Id,
            Name = versionProduct.GetVersionName(),
            VersionNumber = versionProduct.GetNumberVersion(),
            IsObsolete = versionProduct.IsObsolete,
            CreatedAt = versionProduct.CreatedAt,
            CreatedBy = versionProduct.CreatedBy,
            ModifiedAt = versionProduct.ModifiedAt,
            ModifiedBy = versionProduct.ModifiedBy,
            Specifications = versionProduct.Spec.ToDto()
                
        };
    }
    
    //ProductSpecification to ProductSpecificationDto
    public static ProductSpecificationDto ToDto(this ProductSpecification productSpecification)
    {
        return new ProductSpecificationDto
        {
            Id = productSpecification.Id,
            Price = Convert.ToDecimal(Math.Round(productSpecification.GetPrice(),2)),
            Amount = productSpecification.GetAmount(),
            ValueOfProduct = Convert.ToDecimal(Math.Round(productSpecification.GetValueOfProduct(),2)),
            Taxonomy = productSpecification.GetTaxonomy().ToDto(),
            Resources = productSpecification.GetResources()
                .Select(r => r.ToDto())
                .ToList()
        };
    }
    
    //Taxonomy to TaxonomyDto 
    public static TaxonomyDto ToDto(this Taxonomy taxonomy)
    {
        return new TaxonomyDto
        {
            Id = taxonomy.Id,
            ProductCategory = taxonomy.GetCategory().ToString(),
            Generation = taxonomy.GetGeneration().ToString(),
            Kind = taxonomy.GetKind().ToString(),
        };
    }

    public static ResourceDto ToDto(this Resource resource)
    {
        return new ResourceDto
        {
            Key = resource.GetKey(),
            Value = resource.GetValue(),
        };
    }
    
}