using Domain.Interfaces;

namespace Domain.BusinessRules.ProductSpecification;

public class ListOfResourcesMustBeNotEmptyBusinessRule(Domain.ProductSpecification productSpecification) : IBusinessRule
{
    public bool IsBroken => productSpecification.GetResources().Count == 0;
    public string Message => "Resources doesn't exist";
}