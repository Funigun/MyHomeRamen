using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Menu.Offers;

namespace MyHomeRamen.Blazor.Features.Menu.Offers.EditOfferPrice.Models;

public sealed class EditOfferPriceFormModelValidator : BaseValidator<EditOfferPriceFormModel>
{
    public EditOfferPriceFormModelValidator(IStringLocalizer<OffersResources> text)
    {
        RuleFor(model => model.Price)
            .GreaterThan(0).WithMessage(text["PriceRequired"]);
    }
}
