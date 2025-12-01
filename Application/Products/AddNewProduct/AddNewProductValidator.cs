using FluentValidation;

namespace Application.Products.AddNewProduct;

internal class AddNewProductValidator : AbstractValidator<AddNewProduct>
{
    public AddNewProductValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type cannot be empty.");
    }
}