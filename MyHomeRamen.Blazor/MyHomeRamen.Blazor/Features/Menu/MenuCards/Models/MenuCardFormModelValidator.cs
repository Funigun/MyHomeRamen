using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Menu.MenuCards;

namespace MyHomeRamen.Blazor.Features.Menu.MenuCards.Models;

public sealed class MenuCardFormModelValidator : BaseValidator<MenuCardFormModel>
{
    public MenuCardFormModelValidator(IStringLocalizer<MenuCardsResources> text)
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage(text["NameRequired"]);
        RuleFor(model => model.Name)
            .MaximumLength(100).WithMessage(text["NameMaxLength"]);
    }
}
