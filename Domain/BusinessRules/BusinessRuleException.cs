using Domain.Interfaces;

namespace Domain.BusinessRules;

public class BusinessRuleException(IBusinessRule rule) : Exception(rule.Message);