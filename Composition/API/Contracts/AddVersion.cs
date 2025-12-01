namespace Composition.API.Contracts;

/// <summary>
///  Add new version for product with given ID
/// </summary>
/// <param name="Name"> Text </param>
/// <param name="Price">Number with m (decimal), min 1</param>
/// <param name="Amount">Integer, min 1</param>
/// <param name="Resources">typisal json Key:value </param>
/// <param name="ProductCategory"> VirtualMachine / Database / Os / License / Vpn / Firewall / Service </param>
/// <param name="GenerationRecord">G1 / G2 / G2E / G3 </param>
/// <param name="Kind">Specified for ProductCategory):
/// (Intel/Amd) for VirtualMachine,
/// (MicrosoftStd/MicrosoftWeb) for Database,
/// (Windows/Linus) fro OS,
/// (Rds) for License,
/// (OpenVPN) for Vpn,
/// (PfSense/FireGuard) for Firewall,
/// (ServiceImplementation/ExtendedSupport/Backup) for Service. </param>

public record AddVersion(
    string Name,
    decimal Price,
    int Amount,
    Dictionary<string, string> Resources,
    string ProductCategory,
    string GenerationRecord,
    string Kind
);