using Domain.Interfaces;

namespace Domain.BusinessRules;

public class AnyVersionIsActiveBusinessRule(Product product):IBusinessRule
{
    //if we find any active version -> return false on properties IsBroken
    public bool IsBroken => product.Versions.FirstOrDefault(x => x.GetStatusVersion()) == null;
    public string Message => "Product dont have any active version";
}