using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Account;

public static class AccountValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidUserName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty().MaximumLength(50);
    }

    public static IRuleBuilderOptions<T, string> ValidName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty().MaximumLength(50);
    }

    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty().MinimumLength(8);
    }
}
