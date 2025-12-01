using Domain.Interfaces;

namespace Domain.BusinessRules;

internal class ProductMustHaveVersionBusinessRule(Product product) : IBusinessRule
{
    public bool IsBroken => product.Versions.Count == 0;
    public string Message => "Product must have at least one version to be activated.";
}