using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Menu.MenuCards;

namespace MyHomeRamen.Blazor.Features.Menu.MenuCards.Models;

public sealed class MenuSectionFormModelValidator : BaseValidator<MenuSectionFormModel>
{
    public MenuSectionFormModelValidator(IStringLocalizer<MenuCardsResources> text)
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage(text["SectionNameRequired"]);
        RuleFor(model => model.Name)
            .MaximumLength(200).WithMessage(text["SectionNameMaxLength"]);
        RuleFor(model => model.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage(text["SectionSortOrderInvalid"]);
    }
}
