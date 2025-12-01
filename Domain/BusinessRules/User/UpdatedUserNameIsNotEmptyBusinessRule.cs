using Domain.Interfaces;

namespace Domain.BusinessRules.User;

public class UpdatedUserNameIsNotEmptyBusinessRule(string name) : IBusinessRule
{
    public bool IsBroken => !string.IsNullOrWhiteSpace(name) ;
    public string Message => "Name cannot be empty or null";
}