using Domain.Interfaces;

namespace Domain.BusinessRules;

public class VersionMustBeObsoletedToBeActiveAgainBusinessRule(VersionProduct versionProduct) : IBusinessRule
{
    public bool IsBroken => !versionProduct.IsObsolete;
    public string Message => "Version must be obsoleted to become active again.";
}