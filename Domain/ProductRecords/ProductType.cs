using FluentResults;

namespace Domain.ProductRecords
{
    public sealed record ProductType
    {
        public string Name { get; private set; }
        private ProductType() {}
        private ProductType(string name) => Name = name;

        // create directory with product types
        private static readonly Dictionary<string,ProductType> ProductTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["VirtualMachine"] = new ProductType("VirtualMachine"),
            ["Software"] = new ProductType("Software"),
            ["Service"] = new ProductType("Service"),
        };
    
        // public method to create product type
        public static Result<ProductType> Create(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                return Result.Fail<ProductType>("Name cannot be empty.");

            return ProductTypes.TryGetValue(name.Trim(), out var matchProductType) 
                ? Result.Ok(matchProductType) 
                : Result.Fail<ProductType>($"Product type '{name}' was not found.");
        }
    
        public override string ToString() => Name;

        public static List<ProductType> GetProductTypes()
        {
            return ProductTypes.Values.ToList();
        }

        public string GetProductTypeName() => Name;
    
        
    }
}