using Application.ClassDto;
using Application.Contracts;
using Domain;

namespace Application.Users.GetUserById;

public record GetUserById(string Id) : IQuery<UserDto>
{
}