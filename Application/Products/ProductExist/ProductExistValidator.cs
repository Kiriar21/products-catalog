using FluentValidation;

namespace Application.Products.ProductExist;

internal class ProductExistValidator : AbstractValidator<ProductExist>
{
    public ProductExistValidator()
    {
        RuleFor(x => x.Id)
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage("Id product is invalid.");
    }
}