using Application.Contracts;
using Application.ResultFactory;
using Domain;
using Domain.Interfaces;
using FluentResults;

namespace Application.Users.AddNewUser;

internal class AddNewUserHandler: ICommandHandler<AddNewUser>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _repository;

    public AddNewUserHandler(IUnitOfWork unitOfWork, IUserRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }
    public async Task<Result> Handle(AddNewUser command, CancellationToken ct = default)
    {
        var user = User.Register(command.Email, command.Name, command.AuthProvider, command.ProviderSubject);

        if (user.IsFailed)
            return ResultFailFactory.Fail("User can't be created.", ErrCodeEnum.Unexpected, causedBy: user.Errors);

        await _repository.AddAsync(user.Value, ct);

        await _unitOfWork.CommitAsync(ct);
        
        return Result.Ok();
    }
}