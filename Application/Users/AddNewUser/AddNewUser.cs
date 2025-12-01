using Application.Contracts;

namespace Application.Users.AddNewUser;

public record AddNewUser(string Email, string Name, string AuthProvider, string ProviderSubject) : ICommand;