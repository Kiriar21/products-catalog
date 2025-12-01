using Application.ClassDto;
using Application.Contracts;
using Application.Mappers;
using Application.ResultFactory;
using Domain.Interfaces;
using FluentResults;

namespace Application.Users.GetUserById;

internal class GetUserByIdHandler : IQueryHandler<GetUserById, UserDto>
{
    private readonly IUserRepository _repository;

    public GetUserByIdHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserDto>> Handle(GetUserById query, CancellationToken ct = default)
    {
        var userId = Guid.Parse(query.Id);
        var user = await _repository.GetByIdAsync(userId, ct);
       
        if (user == null)
            return ResultFailFactory.Fail<UserDto>("User not found.", ErrCodeEnum.NotFound);

        var userDto = user.ToDto();
        return Result.Ok(userDto);
    }
}