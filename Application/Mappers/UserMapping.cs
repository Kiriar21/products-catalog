using Domain;
using Application.ClassDto;

namespace Application.Mappers;

public static class UserMapping
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            AuthProvider = user.AuthProvider,
            ProviderSubject = user.ProviderSubject,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
        };
    }
}