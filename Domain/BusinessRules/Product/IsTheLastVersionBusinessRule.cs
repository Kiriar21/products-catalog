using Domain.Interfaces;

namespace Domain.BusinessRules;

public class IsTheLastVersionBusinessRule(Product product):IBusinessRule
{
    public bool IsBroken => product.Versions.Count == 1;
    public string Message => "This is the last version of the product.";
}