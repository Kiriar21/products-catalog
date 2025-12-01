namespace Domain.VersionProductRecords;

public sealed record VersionNumber
{
    public int Number { get; private set; }
    private VersionNumber() {}
    private VersionNumber(int number) => Number = number;

    public static VersionNumber CreateNumber(int number) => 
        number <= 0 
            ? throw new ArgumentException("Number cannot be negative.", nameof(number)) 
            : new VersionNumber(number);
    
    public int GetNumber() => Number;
    public override string ToString() => $"Number: {Number}";
}
