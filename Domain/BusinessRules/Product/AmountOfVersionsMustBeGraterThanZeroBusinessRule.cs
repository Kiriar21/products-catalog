using Domain.Interfaces;

namespace Domain.BusinessRules;

public class AmountOfVersionsMustBeGraterThanZeroBusinessRule(Product product): IBusinessRule
{
    public bool IsBroken => product.Versions.Count == 0;
    public string Message => "Product must have any version.";
}