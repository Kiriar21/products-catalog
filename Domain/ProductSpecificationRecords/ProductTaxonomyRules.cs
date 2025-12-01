using Domain.ProductRecords;
using Domain.TaxonomyRecords;
using FluentResults;

namespace Domain.ProductSpecificationRecords
{
    public static class ProductTaxonomyRules
    {
        private static readonly Dictionary<ProductType, ProductCategory[]> AllowedTypes;

        static ProductTaxonomyRules()
        {
            AllowedTypes = new Dictionary<ProductType, ProductCategory[]>();
            
            var virtualMachine = ProductType.Create("VirtualMachine");
            var software = ProductType.Create("Software");
            var service = ProductType.Create("Service");

            if (virtualMachine.IsFailed || software.IsFailed || service.IsFailed)
            {
                Result.Fail("Something went wrong while creating a product types for product rules.");
                return;
            }
            
            var virtualMachineCategory = ProductCategory.Create("VirtualMachine");
            
            var databaseCategory = ProductCategory.Create("Database");
            var firewallCategory = ProductCategory.Create("Firewall");
            var vpnCategory = ProductCategory.Create("Vpn");
            var licenseCategory = ProductCategory.Create("License");
            var osCategory = ProductCategory.Create("Os");
            
            var serviceCategory = ProductCategory.Create("Service");
            
            if (virtualMachine.IsFailed || databaseCategory.IsFailed || firewallCategory.IsFailed
                || vpnCategory.IsFailed || licenseCategory.IsFailed || osCategory.IsFailed 
                || serviceCategory.IsFailed)
            {
                Result.Fail("Something went wrong while creating a product categories for product rules.");
                return;
            }
            
            AllowedTypes[virtualMachine.Value] = new[] {virtualMachineCategory.Value};
            AllowedTypes[software.Value] = new[] {databaseCategory.Value, firewallCategory.Value, vpnCategory.Value, licenseCategory.Value, osCategory.Value};
            AllowedTypes[service.Value] = new[] {serviceCategory.Value};
            
        }
        //return status allowed to assing category to selected product type
        public static Result IsAllowed(ProductType? productType, ProductCategory? productCategory)
        {
            if(productType == null) 
                return Result.Fail("Product type is empty!!!.");
        
            if(productCategory == null) 
                return Result.Fail("Product Category is empty.");
            
            if(!AllowedTypes.TryGetValue(productType, out var allowedCategory)) 
                return Result.Fail($"Product Type {productType} is not allowed.");

            return allowedCategory.Contains(productCategory)
                ? Result.Ok()
                : Result.Fail($"Product Category {productCategory} is not allowed in Product Type {productType}.");
            
        }
            
        //return allowed categories for type
        public static Result<IEnumerable<ProductCategory>> GetAllowedCategories(ProductType productType)
        {
            return !AllowedTypes.TryGetValue(productType, out var allowedCategory) 
                ? Result.Fail("Product Type is not allowed.") 
                : Result.Ok<IEnumerable<ProductCategory>>(allowedCategory);
        }
    
        //return allowed categories
        public static Result<IEnumerable<ProductType>> GetAllowedTypes() =>
            Result.Ok<IEnumerable<ProductType>>(AllowedTypes.Keys.ToList());
    }
    
}
