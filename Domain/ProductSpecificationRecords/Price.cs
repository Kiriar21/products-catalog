using FluentResults;

namespace Domain.ProductSpecificationRecords;
public sealed record Price
{
    public decimal Value { get; private set; }
    private Price() {}
    private Price(decimal price) => Value = price;

    public static Result<Price> CreatePrice(decimal price) =>
        price > 0
            ? Result.Ok(new Price(price))
            : Result.Fail<Price>("Price must be greater than 0.");
    public decimal GetPrice() => Value;
} 
