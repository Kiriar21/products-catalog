using FluentValidation;

namespace Application.Products.GetPaginatedProducts;

internal class GetPaginatedProductsValidator :AbstractValidator<GetPaginatedProducts>
{
    public GetPaginatedProductsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be in range 1 and 100.");
    }
}