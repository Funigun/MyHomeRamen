using FluentValidation;
using FluentValidation.Results;

namespace MyHomeRamen.Blazor.Features.Admin.Employees.CreateEmployee;

public sealed class CreateEmployeeValidator : AbstractValidator<CreateEmployeeFormModel>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.TemporaryPassword)
            .NotEmpty()
            .MinimumLength(8);
    }

    // MudBlazor per-field validation adapter
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
        async (model, propertyName) =>
        {
            ValidationResult result = await ValidateAsync(
                ValidationContext<CreateEmployeeFormModel>.CreateWithOptions(
                    (CreateEmployeeFormModel)model,
                    opts => opts.IncludeProperties(propertyName)));

            return result.IsValid
                ? []
                : result.Errors.Select(e => e.ErrorMessage);
        };
}
