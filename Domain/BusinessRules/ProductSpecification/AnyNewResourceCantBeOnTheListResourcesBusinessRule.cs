using Domain.Interfaces;

namespace Domain.BusinessRules.ProductSpecification;

public class AnyNewResourceCantBeOnTheListResourcesBusinessRule : IBusinessRule
{
    private readonly List<Resource> _newResources;
    private readonly HashSet<string> _existingKeys;

    public AnyNewResourceCantBeOnTheListResourcesBusinessRule(Domain.ProductSpecification productSpecification, List<Resource> newResources)
    {
        var resources = productSpecification.GetResources();
        _newResources = newResources;
        _existingKeys = resources
                        .Select(x => x.GetKey())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public bool IsBroken => _newResources.Any(x => _existingKeys.Contains(x.GetKey()));
    
    public string Message => "One or more resources do exist in the list.";
}