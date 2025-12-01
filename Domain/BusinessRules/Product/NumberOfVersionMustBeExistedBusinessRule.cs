using Domain.Interfaces;

namespace Domain.BusinessRules;

public class NumberOfVersionMustBeExistedBusinessRule(Product product, int number): IBusinessRule
{
    public bool IsBroken => product.Versions.FirstOrDefault(x => x.GetNumberVersion() == number) == null;
    public string Message => "Number of version not exist.";
}