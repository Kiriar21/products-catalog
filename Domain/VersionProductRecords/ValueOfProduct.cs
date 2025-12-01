using Domain.ProductSpecificationRecords;

namespace Domain.VersionProductRecords;

public sealed record ValueOfProduct
{
    private decimal Value { get; }
    private ValueOfProduct(decimal value) => this.Value = value;

    public static ValueOfProduct CreateAmount(Price price, Amount amount) =>
        new(price.GetPrice() * amount.GetAmount());

    public decimal GetValueOfProduct() => Value;
} 
