using Domain.Interfaces;

namespace Domain.BusinessRules;

public class ActivateNumberVersionMustBeGreaterThanZeroBusinessRule(int number):IBusinessRule
{
    public bool IsBroken => number <= 0;
    public string Message => "Number must be greater than zero";
}