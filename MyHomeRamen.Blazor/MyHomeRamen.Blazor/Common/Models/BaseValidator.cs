using FluentValidation;

namespace MyHomeRamen.Blazor.Common.Models;

public abstract class BaseValidator<TModel> : AbstractValidator<TModel>
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        FluentValidation.Results.ValidationResult result = await ValidateAsync(
            ValidationContext<TModel>.CreateWithOptions(
                (TModel)model,
                x => x.IncludeProperties(propertyName)));

        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
    };
}
