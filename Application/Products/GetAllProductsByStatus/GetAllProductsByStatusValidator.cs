using FluentValidation;

namespace Application.Products.GetAllProductsByStatus;

internal class GetAllProductsByStatusValidator : AbstractValidator<GetAllProductsByStatus>
{
    public GetAllProductsByStatusValidator()
    {
        RuleFor(x=>x.Status)
            .NotEmpty().WithMessage("Status is required.");
    }
}