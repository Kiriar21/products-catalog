using Domain.Interfaces;

namespace Domain.BusinessRules;

public class VersionMustBeActiveToBecomeObsoletedBusinessRule(VersionProduct versionProduct) : IBusinessRule
{
    public bool IsBroken => versionProduct.IsObsolete;
    public string Message => "Version must be active";
}