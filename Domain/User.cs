using Domain.BusinessRules.User;
using Domain.Interfaces;
using FluentResults;

namespace Domain;

public sealed class User  : EntityBase<Guid>, IAggregateRoot
{
    public string Email { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string AuthProvider { get; private set; } = null!;      
    public string ProviderSubject { get; private set; } = null!;   
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    private User():base(Guid.NewGuid()){}

    private User(string email, string displayName, string provider, string providerSubject) : base(Guid.NewGuid())
    {
        Email = email;
        DisplayName = displayName;
        AuthProvider = provider;
        ProviderSubject = providerSubject;
    }

    public static Result<User> Register(string email, string displayName, string provider, string providerSubject)
    {
        var user = new User(email, displayName, provider, providerSubject);
        return Result.Ok(user);
    }
    public void Deactivate() => IsActive = false;
}