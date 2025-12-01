using Domain.Interfaces;

namespace Domain.BusinessRules;

internal class AllOfProductVersionsMustBeObsoletedBusinessRule(Product product) : IBusinessRule
{
    public bool IsBroken => product.Versions.Any(x => !x.IsObsolete); 
    
    public string Message => "Product cannot has any versions that are already activated.";
}