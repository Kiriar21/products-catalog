using FluentValidation;
using System;
using FluentValidation.Validators;

namespace Application.Products.GetProductById;

internal class GetProductByIdValidator : AbstractValidator<GetProductById>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x.Id)
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage("Id product is invalid.");
    }
}