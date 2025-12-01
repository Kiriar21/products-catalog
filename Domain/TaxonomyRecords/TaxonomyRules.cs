using FluentResults;

namespace Domain.TaxonomyRecords;

public static class TaxonomyRules
{
    private static readonly Dictionary<ProductCategory, Kind[]> AllowedKinds;
    
    // list of kinds for each of category
    static TaxonomyRules()
    {
        AllowedKinds = new Dictionary<ProductCategory, Kind[]>();

        //vm
        var vmVirtualMachine = ProductCategory.Create("VirtualMachine");
        var amd = Kind.Create("Amd");
        var intel = Kind.Create("Intel");
        if (vmVirtualMachine.IsFailed || amd.IsFailed || intel.IsFailed)
        {
            Result.Fail("Something went wrong, while creating a VirtualMachine records in Taxonomy Rules.");
            return;
        }
        AllowedKinds[vmVirtualMachine.Value] = [amd.Value, intel.Value];

        //Database
        var vmDatabase = ProductCategory.Create("Database");
        var microsoftStd = Kind.Create("MicrosoftStd");
        var microsoftWeb = Kind.Create("MicrosoftWeb");
        if (vmDatabase.IsFailed || microsoftStd.IsFailed || microsoftWeb.IsFailed)
        {
            Result.Fail("Something went wrong, while creating a Database records in Taxonomy Rules.");
            return;
        }
        AllowedKinds[vmDatabase.Value] = [microsoftStd.Value, microsoftWeb.Value];
        
        //OS 
        var vmOs = ProductCategory.Create("Os");
        var windows = Kind.Create("Windows");
        var linux = Kind.Create("Linux");
        if (vmOs.IsFailed || windows.IsFailed || linux.IsFailed)
        {
            Result.Fail("Something went wrong, while creating a OS records in Taxonomy Rules.");
            return;
        }
        AllowedKinds[vmOs.Value] = [linux.Value, windows.Value];
        
        //license
        var vmLicense = ProductCategory.Create("License");
        var rds = Kind.Create("Rds");
        if (vmLicense.IsFailed || rds.IsFailed)
        {
            Result.Fail("Something went wrong, while creating a License records in Taxonomy Rules.");
            return;
        }
        AllowedKinds[vmLicense.Value] = [rds.Value];

        //vpn
        var vmVpn = ProductCategory.Create("Vpn");
        var openVpn = Kind.Create("OpenVpn");
        if (vmVpn.IsFailed || openVpn.IsFailed)
        {
            Result.Fail("Something went wrong, while creating a Vpn records in Taxonomy Rules.");
            return;
        }
        AllowedKinds[vmVpn.Value] = [openVpn.Value];


        //firewalle
        var vmFirewall = ProductCategory.Create("Firewall");
        var fireGuard = Kind.Create("FireGuard");
        var pfSense = Kind.Create("PfSense");
        if (vmFirewall.IsFailed || pfSense.IsFailed || fireGuard.IsFailed)
        {
            Result.Fail("Something went wrong, while creating a Firewall records in Taxonomy Rules.");
            return;
        }
        AllowedKinds[vmFirewall.Value] = [fireGuard.Value, pfSense.Value];

        //services
        var vmService = ProductCategory.Create("Service");
        var backup = Kind.Create("Backup");
        var extendedSupport = Kind.Create("ExtendedSupport");
        var serviceImplementation = Kind.Create("ServiceImplementation");
        if (vmService.IsFailed || serviceImplementation.IsFailed || extendedSupport.IsFailed || backup.IsFailed)
        {
            Result.Fail("Something went wrong, while creating a Service records in Taxonomy Rules.");
            return;
        }
        AllowedKinds[vmService.Value] = [backup.Value, extendedSupport.Value, serviceImplementation.Value];

    }

    //return status allowed to assign kind for category
    public static Result IsAllowed(ProductCategory? productCategory, Kind? kind)
    { 
        if(productCategory == null) 
            return Result.Fail("Product Category is empty.");
        
        if(kind == null) 
            return Result.Fail("Kind is empty.");
        
        if(!AllowedKinds.TryGetValue(productCategory, out var allowedKind)) 
            return Result.Fail($"Product Category {productCategory} is not allowed.");
        
        return allowedKind.Contains(kind)
            ? Result.Ok()
            : Result.Fail($"Kind {kind} is not allowed in Product Category {productCategory}.");
    }
        
    
    //return allowed kinds for category
    public static Result<IEnumerable<Kind>> GetAllowedKinds(ProductCategory productCategory)
    {
        return !AllowedKinds.TryGetValue(productCategory, out var kind) 
            ? Result.Fail("Product Category is not allowed.") 
            : Result.Ok<IEnumerable<Kind>>(kind);
    }
    
    //return allowed categories
    public static Result<IEnumerable<ProductCategory>> GetAllowedCategories() =>
        Result.Ok<IEnumerable<ProductCategory>>(AllowedKinds.Keys.ToList());
}