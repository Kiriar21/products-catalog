using FluentResults;

namespace Domain.TaxonomyRecords;

public sealed record ProductCategory
{
    public string Name { get; private set; }
    private ProductCategory() {}
    private ProductCategory(string name) => Name = name;

    // create directory with product types
    private static readonly Dictionary<string,ProductCategory> ProductCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        ["VirtualMachine"] = new ProductCategory("VirtualMachine"),
        ["Database"] = new ProductCategory("Database"),
        ["Os"] = new ProductCategory("Os"),
        ["License"] = new ProductCategory("License"),
        ["Vpn"] = new ProductCategory("Vpn"),
        ["Firewall"] = new ProductCategory("Firewall"),
        ["Service"] = new ProductCategory("Service"),
    };
    
    // public method to create product type
    public static Result<ProductCategory> Create(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            return Result.Fail<ProductCategory>("Name product category cannot be empty.");

        return ProductCategories.TryGetValue(name.Trim(), out var matchProductCategory) 
            ? Result.Ok(matchProductCategory) 
            : Result.Fail<ProductCategory>($"Product category type '{name}' was not found.");
    }
    
    public override string ToString() => Name;

    public static List<ProductCategory> GetProductCategories()
    {
        return ProductCategories.Values.ToList();
    }

    public string GetProductCategoryName() => Name;
    
        
}