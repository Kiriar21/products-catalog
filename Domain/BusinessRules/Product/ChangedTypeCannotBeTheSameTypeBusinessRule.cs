using Domain.Interfaces;
using Domain.ProductRecords;

namespace Domain.BusinessRules;

public class ChangedTypeCannotBeTheSameTypeBusinessRule(Product product, ProductType productType): IBusinessRule
{
    public bool IsBroken => product.GetProductType == productType;
    public string Message => "Product type must be one of the type the product but not the same.";
    
}