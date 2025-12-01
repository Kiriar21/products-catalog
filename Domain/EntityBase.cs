using System.Text;
using Domain.Interfaces;
using FluentResults;

namespace Domain;

public abstract class EntityBase<TId>
{
    public TId Id { get; private set; }

    internal EntityBase(TId id)
    {
        Id = id;
    }

    protected Result ValidateBusinessRule(params IBusinessRule[] rules)
    {
        var builder = new StringBuilder();
        
        foreach (var rule in rules)
        {
            if (rule.IsBroken)
                builder.Append($" {rule.Message}");
        }
        
        return builder.Length == 0 ? Result.Ok() : Result.Fail(builder.ToString());
    }
}