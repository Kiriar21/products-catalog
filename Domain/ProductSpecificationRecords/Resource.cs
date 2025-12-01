using FluentResults;

namespace Domain;

public sealed record Resource
{
    public string Key { get; private set; }
    public string Value {get; private set; }
    
    private Resource(){}
    private Resource(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public static Result<Resource> CreateResource(string key, string value)
    {
        
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result.Fail<Resource>("Resource key cannot be null or whitespace");
        }
        
        return string.IsNullOrEmpty(value) ? Result.Fail<Resource>("Resource value cannot be null.") : Result.Ok(new Resource(key, value));
    }
    
    public string GetKey() => Key;
    public string GetValue() => Value;
    public override string ToString() => $"Name: {Key}, Value: {Value}";

    public bool HasKey(string otherKey, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        => Key.Equals(otherKey, stringComparison);

    
}