using Domain.Interfaces;

namespace Domain.BusinessRules;

public class NameCannotBeEmptyBusinessRule(string name) : IBusinessRule
{
    public bool IsBroken => name.Length <= 0;
    public string Message => "Name cannot be empty.";
}