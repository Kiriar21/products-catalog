using Domain.Interfaces;

namespace Domain.BusinessRules;

public class VersionAboutNumberIsActiveBusinessRule(Product product, int number):IBusinessRule
{
    public bool IsBroken => (bool)(!product.Versions.FirstOrDefault(x => x.GetNumberVersion() == number)?.GetStatusVersion())!;
    
    public string Message =>  "Product version with this number is not active version.";
}