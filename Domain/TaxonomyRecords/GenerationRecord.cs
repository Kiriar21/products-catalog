using FluentResults;

namespace Domain.TaxonomyRecords;

public sealed record GenerationRecord
{
    public string Name { get; private set; }

    private GenerationRecord() { }
    private GenerationRecord(string name) => Name = name;

    // create directory with product types
    private static readonly Dictionary<string, GenerationRecord> Generations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["G1"] = new GenerationRecord("G1"),
        ["G2"] = new GenerationRecord("G2"),
        ["G2E"] = new GenerationRecord("G2E"),
        ["G3"] = new GenerationRecord("G3"),
    };

    // public method to create product type
    public static Result<GenerationRecord> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail<GenerationRecord>("Name generation cannot be empty.");

        return Generations.TryGetValue(name.Trim(), out var matchGeneration)
            ? Result.Ok(matchGeneration)
            : Result.Fail<GenerationRecord>($"Product generation type '{name}' was not found.");
    }

    public override string ToString() => Name;

    public static List<GenerationRecord> GetProductTypes()
    {
        return Generations.Values.ToList();
    }

    public string GetProductTypeName() => Name;
}
