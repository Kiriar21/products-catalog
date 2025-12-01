using FluentResults;

namespace Domain.VersionProductRecords;
public sealed record VersionName 
{
    public string Name { get; private set; }
    private VersionName() {}
    private VersionName(string name) => Name = name;
    public static Result<VersionName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail<VersionName>("Name cannot be null or whitespace.");
        
        var nameTrimmed = name.Trim();
        
        return nameTrimmed.Trim().Length is < 3 or > 30
        ? Result.Fail("Name must be between 3 and 30 characters.")
            : Result.Ok(new VersionName(nameTrimmed));
    }
    
    public string GetName() => Name;
    public override string ToString() => $"Name: {Name}";
    
}