using Domain.BusinessRules.ProductSpecification;
using Domain.ProductRecords;
using Domain.ProductSpecificationRecords;
using Domain.TaxonomyRecords;
using Domain.VersionProductRecords;
using FluentResults;

namespace Domain;

//class of Product Specification, inherit from EntityBase (ID, Validate forms)
public class ProductSpecification:EntityBase<Guid>
{
    // =============== ProductSpecification Properties ===============
    
    // basic properties
    public Price Price { get; private set; }
    public Taxonomy Taxonomy { get; private set; } 
    public Amount Amount { get; private set; }
    private ValueOfProduct ValueOfProduct { get; set; }

    private readonly List<Resource> _resources = [];
    private IReadOnlyList<Resource> Resources => _resources.AsReadOnly();

    // ====================================================================
    // =============== Building ProductSpecification ======================
    // ====================================================================


    //EF
    private ProductSpecification():base(Guid.NewGuid()){}
    
    //basic constructor
    private ProductSpecification(Price price, Taxonomy taxonomy, Amount amount) : base(Guid.NewGuid())
    {
        Price = price;
        Taxonomy = taxonomy;
        Amount = amount;
        ValueOfProduct = ValueOfProduct.CreateAmount(Price, Amount);
    }

    // generate new product specification
    public static Result<ProductSpecification> GenerateProductSpecification(decimal price, Taxonomy taxonomy, int amount, IDictionary<string, string>? resources = null)
    {
        var tempAmount = Amount.Create(amount);
        
        if(tempAmount.IsFailed)
            return Result.Fail<ProductSpecification>(tempAmount.Errors);

        var tempPrice = Price.CreatePrice(price);
        
        if(tempPrice.IsFailed)
            return Result.Fail<ProductSpecification>(tempPrice.Errors);
        
        var spec = new ProductSpecification(tempPrice.Value, taxonomy, tempAmount.Value);

        if (resources is null) return Result.Ok(spec);
        
        foreach (var r in resources)
        {
            var addResult = spec.AddResource(r.Key, r.Value);
            if (addResult.IsFailed)
                return Result.Fail<ProductSpecification>(addResult.Errors);
        }

        return Result.Ok(spec);
    }
    
    // ================================================================
    // =============== Basic Method for ProductSpecification ======================
    // ================================================================

    
    // get price for component
    public decimal GetPrice() => Price.GetPrice();

    // get amount of component
    public int GetAmount() => Amount.GetAmount();
    
    // get price of product
    public decimal GetValueOfProduct()
    {
        if (Price == null || Amount == null)
            return 0;
    
        return (decimal)(Price.Value * Amount.Value);
    }
    
    // get taxonomy of product
    public Taxonomy GetTaxonomy() => Taxonomy;
    
    //return allowed categories for product type
    public static Result<IEnumerable<ProductCategory>> GetAllowedCategories(ProductType productType)
    {
        var x = ProductTaxonomyRules.GetAllowedCategories(productType);
        return x.IsFailed ?  Result.Fail<IEnumerable<ProductCategory>>(x.Errors) : Result.Ok(x.Value);
    }   
    
    //return allowed name of categories for product type
    public static Result<IEnumerable<string>> GetAllowedKindsNames(ProductType productType)
    {
        var x = GetAllowedCategories(productType);
        return x.IsFailed ?
            Result.Fail<IEnumerable<string>>(x.Errors)
            : Result.Ok(x.Value.Select(category => category.ToString()));
    }
    
    // ================================================================
    // =============== Building Resources =============================
    // ================================================================

    //creating new Resource
    private static Result<Resource> CreateResource(string key, string value) => Resource.CreateResource(key, value);

    
    // ================================================================
    // =============== Basic aggregate for Resources ==================
    // ================================================================
    
    
    //add new resource using data of key and value
    public Result AddResource(string key, string value)
    {
        // test for non exist the same key on the list
        var result = ValidateBusinessRule(new FindTheSameResourceNameKeyToAddBusinessRule(this, key));

        if (result.IsFailed)
            return result;
        
        var newResource = CreateResource(key, value);
        
        if(newResource.IsFailed)
            return Result.Fail(newResource.Errors);
        
        _resources.Add(newResource.Value);
        
        return Result.Ok();
    }
    
    //add new resource object
    public Result AddResource(Resource resource)
    {
        // test for list of Resources cannot be empty
        var result = ValidateBusinessRule(new FindTheSameResourceNameKeyToAddBusinessRule(this, resource.GetKey()));
    
        if (result.IsFailed)
            return result;
        
        _resources.Add(resource);
        
        return result;
    }
    
    //add new list of resources
    public Result AddResources(List<Resource> resources)
    {
        // test for list of Resources cannot be empty
        var result = ValidateBusinessRule(new AnyNewResourceCantBeOnTheListResourcesBusinessRule(this, resources));
        
        if (result.IsFailed)
            return result;
        
        _resources.AddRange(resources);
        
        return result;
    }
    
    //remove one of resource as a key name
    public Result RemoveResource(string key)
    {
        // test for non exist the same key on the list
        var result = ValidateBusinessRule(new FindTheSameResourceNameKeyToRemoveBusinessRule(this, key));

        if (!result.IsFailed)
            return result;

        var resource = FindResource(key);
        
        if(resource.IsSuccess)
            _resources.Remove(resource.Value);
        
        return result;
    }
    
    //remove one of resource as Object
    public Result RemoveResource(Resource resource)
    {
        // test for non exist the same key on the list
        var result = ValidateBusinessRule(new FindTheSameResourceNameKeyToRemoveBusinessRule(this, resource.GetKey()));

        if (!result.IsFailed)
            return result;
        
        var findResource = FindResource(resource.GetKey());
        if (findResource.IsSuccess) _resources.Remove(findResource.Value);

        return result;
    }
    
    // public Result RemoveAllResources()
    public Result RemoveAllResources()
    {
        var result = ValidateBusinessRule(new ListOfResourcesMustBeNotEmptyBusinessRule(this));

        if (result.IsFailed)
            return result;
        
        _resources.Clear();
        
        return result;
    }
     
    // ================================================================
    // =============== Basic Methods for Resources ====================
    // ================================================================
    
    // find the product on the list using name
    public Resource? GetResourceOfProduct(string key) 
        => Resources.FirstOrDefault(x => x.HasKey(key));
    
    // get all of resource
    public List<Resource> GetResources() => Resources.Count == 0 ? [] : _resources.ToList();
    
    // return value for key resource
    public object? GetValueOfResource(string key) 
        => Resources.FirstOrDefault(x => x.HasKey(key))?.GetValue();
    
    // return info about key and value using object
    public static (string Key, object Value) GetResourcesOfProduct(Resource resource) => (resource.GetKey(), resource.GetValue());
    
    // find the resource as a name 
    private Result<Resource> FindResource(string key) 
        => _resources.FirstOrDefault(x => x.HasKey(key)) 
        ?? Result.Fail<Resource>("Resource not found");
    
        
}


