using FluentResults;

namespace Domain.TaxonomyRecords;

public sealed record Kind
{
    public string Name { get; private set; }

    private Kind(){ }
    private Kind(string name) => Name = name;

    // create directory with product types
    private static readonly Dictionary<string,Kind> Kinds = new(StringComparer.OrdinalIgnoreCase)
    {
        // VirtualMachine,
        ["Intel"] = new Kind("Intel"),
        ["Amd"] = new Kind("Amd"),
        // Database,
        ["MicrosoftStd"] = new Kind("MicrosoftStd"),
        ["MicrosoftWeb"] = new Kind("MicrosoftWeb"),
        // OS,
        ["Windows"] = new Kind("Windows"),
        ["Linux"] = new Kind("Linux"),
        // License,
        ["Rds"] = new Kind("Rds"),
        // Vpn,
        ["OpenVpn"] = new Kind("OpenVpn"),
        // Firewall,
        ["PfSense"] = new Kind("PfSense"),
        ["FireGuard"] = new Kind("FireGuard"),
        // Service
        ["ServiceImplementation"] = new Kind("ServiceImplementation"),
        ["ExtendedSupport"] = new Kind("ExtendedSupport"),
        ["Backup"] = new Kind("Backup"),
    };
    
    // public method to create product type
    public static Result<Kind> Create(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            return Result.Fail<Kind>("Name kind cannot be empty.");

        return Kinds.TryGetValue(name.Trim(), out var matchKind) 
            ? Result.Ok(matchKind) 
            : Result.Fail<Kind>($"Product kind type '{name}' was not found.");
    }
    
    public override string ToString() => Name;

    public static List<Kind> GetKinds()
    {
        return Kinds.Values.ToList();
    }

    public string GetKindName() => Name;
    
        
}