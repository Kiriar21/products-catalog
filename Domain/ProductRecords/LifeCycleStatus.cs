using FluentResults;

namespace Domain.ProductRecords;

public sealed record LifeCycleStatus
{
    public string Name { get; private set; }
    private LifeCycleStatus() {}
    private LifeCycleStatus(string name) => Name = name;
    
    // create directory with life cycles tatus
    private static readonly Dictionary<string,LifeCycleStatus> LifeCycleStatusTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Draft"] = new LifeCycleStatus("Draft"),
        ["Active"] = new LifeCycleStatus("Active"),
        ["Archived"] = new LifeCycleStatus("Archived"),
    };
    
    // public method to create life cycle status
    public static Result<LifeCycleStatus> Create(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            return Result.Fail<LifeCycleStatus>("Life status cannot be empty.");

        return LifeCycleStatusTypes.TryGetValue(name.Trim(), out var matchProductType) 
            ? Result.Ok(matchProductType) 
            : Result.Fail<LifeCycleStatus>($"Life cycle status type '{name}' was not found.");
    }
    
    public override string ToString() => Name;

    public static List<LifeCycleStatus> GetLifeCycleStatuses()
    {
        return LifeCycleStatusTypes.Values.ToList();
    }

    public string GetProductTypeName() => Name;
}