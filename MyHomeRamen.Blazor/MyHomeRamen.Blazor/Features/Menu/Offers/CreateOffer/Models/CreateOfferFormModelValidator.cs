using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Menu.Offers;

namespace MyHomeRamen.Blazor.Features.Menu.Offers.CreateOffer.Models;

public sealed class CreateOfferFormModelValidator : BaseValidator<CreateOfferFormModel>
{
    public CreateOfferFormModelValidator(IStringLocalizer<OffersResources> text)
    {
        RuleFor(model => model.ProductId)
            .NotEmpty().WithMessage(text["NameRequired"]);
        RuleFor(model => model.Price)
            .GreaterThan(0).WithMessage(text["PriceRequired"]);
    }
}
