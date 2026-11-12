using MudBlazor;
using MudBlazor.Services;
using MyHomeRamen.Blazor.Features.Menu.MenuCards.Components;
using MyHomeRamen.Blazor.Features.Menu.MenuCards.Models;

namespace MyHomeRamen.BlazorTests.MenuCards;

public sealed class MenuCardOfferAssignmentDialogTests : Bunit.BunitContext
{
    [Fact]
    public async Task AssignmentDialog_ShouldSearchAndPreserveMultipleSelections()
    {
        Services.AddMudServices();
        Services.AddLocalization();
        JSInterop.Mode = JSRuntimeMode.Loose;
        IRenderedComponent<MudDialogProvider> provider = Render<MudDialogProvider>();

        Guid misoId = Guid.NewGuid();
        Guid shoyuId = Guid.NewGuid();
        MenuCardOfferChoiceViewModel[] offers =
        [
            CreateOffer(misoId, "Miso ramen", "Soybean broth", "Noodles"),
            CreateOffer(shoyuId, "Shoyu ramen", "Soy sauce broth", "Classics")
        ];
        DialogParameters<MenuCardOfferAssignmentDialog> parameters = new()
        {
            { dialog => dialog.EligibleOffers, offers }
        };
        IDialogService dialogService = Services.GetRequiredService<IDialogService>();
        IDialogReference dialog = await dialogService.ShowAsync<MenuCardOfferAssignmentDialog>(
            "Assign offers",
            parameters);
        IRenderedComponent<MenuCardOfferAssignmentDialog> component =
            provider.FindComponent<MenuCardOfferAssignmentDialog>();

        AngleSharp.Dom.IElement searchInput = component.Find("input[type='text']");
        await searchInput.InputAsync("Miso");

        Assert.Contains("Miso ramen", component.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Shoyu ramen", component.Markup, StringComparison.Ordinal);

        await component.Find("input[type='text']").InputAsync(string.Empty);
        IReadOnlyList<AngleSharp.Dom.IElement> checkboxes = component.FindAll("input[type='checkbox']");
        Assert.Equal(2, checkboxes.Count);
        await checkboxes[0].ChangeAsync(true);
        await checkboxes[1].ChangeAsync(true);
        await component.FindAll("button")
            .Single(button => button.TextContent.Contains("Add selected offers", StringComparison.Ordinal))
            .ClickAsync();

        DialogResult? result = await dialog.Result;
        Assert.NotNull(result);
        Assert.False(result.Canceled);
        Guid[] selectedOfferIds = Assert.IsType<Guid[]>(result.Data);
        Assert.Equal(2, selectedOfferIds.Length);
        Assert.Contains(misoId, selectedOfferIds);
        Assert.Contains(shoyuId, selectedOfferIds);
    }

    private static MenuCardOfferChoiceViewModel CreateOffer(
        Guid offerId,
        string name,
        string description,
        string category)
        => new(
            offerId,
            name,
            description,
            string.Empty,
            [category],
            12m,
            true,
            "Active",
            "Active");
}
