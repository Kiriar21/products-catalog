using Domain.Interfaces;
using Domain.ProductRecords;

namespace Domain.BusinessRules;

public class ChangedTypeMustBeOnTheListOfProductTypesBusinessRule(ProductType productType) : IBusinessRule
{
    public bool IsBroken => !ProductType.GetProductTypes().Contains(productType);
    public string Message => "Product type must be one of the type the product.";
}