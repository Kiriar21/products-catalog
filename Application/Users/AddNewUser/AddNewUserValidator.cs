using FluentValidation;

namespace Application.Users.AddNewUser;

internal class AddNewUserValidator : AbstractValidator<AddNewUser>
{
    public AddNewUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.AuthProvider).NotEmpty().WithMessage("AuthProvider is required.");
        RuleFor(x => x.ProviderSubject).NotEmpty().WithMessage("ProviderSubject is required.");
    }
}