using FluentResults;

namespace Domain.ProductSpecificationRecords;

public sealed record Amount
{
    public int Value { get; private set; }
    private Amount() {}
    private Amount(int amount) => this.Value = amount;

    public static Result<Amount> Create(int amount) =>
        amount > 0 
            ? Result.Ok(new Amount(amount))
            : Result.Fail<Amount>("Amount must be greater than 0.");
    
    public int GetAmount() => Value;
} 