using FluentValidation;

namespace Application.Products.GetProductsWithPriceRange;

internal class GetProductsWithPriceRangeValidator: AbstractValidator<GetProductsWithPriceRange>
{
    public GetProductsWithPriceRangeValidator()
    {
        RuleFor(x=>x.PriceMin)
            .GreaterThan(0)
            .WithMessage("Price minimum must be greater than 0.");
        
        RuleFor(x=>x.PriceMax)
            .GreaterThan(0)
            .WithMessage("Price maximum must be greater than 0.");
        
        RuleFor(x => x.PriceMin)
            .LessThanOrEqualTo(x => x.PriceMax)
            .WithMessage("Minimum price must be less than or equal to maximum price.");

        
    }
}