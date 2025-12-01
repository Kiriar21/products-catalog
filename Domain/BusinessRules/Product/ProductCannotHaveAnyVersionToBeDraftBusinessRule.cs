using Domain.Interfaces;

namespace Domain.BusinessRules;

public class ProductCannotHaveAnyVersionToBeDraftBusinessRule(Product product) : IBusinessRule
{
    public bool IsBroken => product.Versions.Count != 0;
    public string Message => "Product cannot have any versions on list.";
}