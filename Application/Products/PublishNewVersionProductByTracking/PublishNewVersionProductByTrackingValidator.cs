using FluentValidation;

namespace Application.Products.PublishNewVersionProductByTracking;

internal class PublishNewVersionProductByTrackingValidator : AbstractValidator<PublishNewVersionProductByTracking>
{
    public PublishNewVersionProductByTrackingValidator()
    {
        
        RuleFor(x => x.IdProduct)
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage("Id product is invalid.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required and cannot be empty.")
            .NotEqual(string.Empty).WithMessage("Name is required and cannot be string empty.")
            .NotNull().WithMessage("Name is required and cannot be null..");
        
        RuleFor(x => x.ProductCategory)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required and cannot be empty.")
            .NotEqual(string.Empty).WithMessage("Name is required and cannot be string empty.")
            .NotNull().WithMessage("Name is required and cannot be null..");
        
        RuleFor(x => x.GenerationRecord)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required and cannot be empty.")
            .NotEqual(string.Empty).WithMessage("Name is required and cannot be string empty.")
            .NotNull().WithMessage("Name is required and cannot be null..");
        
        RuleFor(x => x.Kind)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required and cannot be empty.")
            .NotEqual(string.Empty).WithMessage("Name is required and cannot be string empty.")
            .NotNull().WithMessage("Name is required and cannot be null..");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");


    }
}