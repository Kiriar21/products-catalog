using Domain.Interfaces;

namespace Domain.BusinessRules;

public class NameOfTheVersionCannotBeEmpty(string name): IBusinessRule
{
    public bool IsBroken => name.Length <= 0;
    public string Message => "Name of the version cannot be empty";
    
}