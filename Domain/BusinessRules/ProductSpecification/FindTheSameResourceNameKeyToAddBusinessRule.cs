using Domain.Interfaces;

namespace Domain.BusinessRules.ProductSpecification;

public class FindTheSameResourceNameKeyToAddBusinessRule(Domain.ProductSpecification productSpecification, string key):IBusinessRule
{
    private readonly List<Resource> _x = productSpecification.GetResources();
    public bool IsBroken => _x.Any(y => y.HasKey(key));
    public string Message => "Name of the resource doesn't exist";
}