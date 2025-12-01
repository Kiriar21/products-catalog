namespace Domain.Interfaces;

public interface IBusinessRule
{
    public bool IsBroken { get; }
    public string Message { get; }
}