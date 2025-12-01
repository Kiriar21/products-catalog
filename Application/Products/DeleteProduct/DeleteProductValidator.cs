using FluentAssertions;
using FluentValidation;

namespace Application.Products.DeleteProduct;

internal class DeleteProductValidator : AbstractValidator<DeleteProduct>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.Id)
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage("Id product is invalid.");
    }
}