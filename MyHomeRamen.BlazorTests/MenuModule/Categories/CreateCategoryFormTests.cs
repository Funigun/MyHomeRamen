using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MyHomeRamen.Blazor.Features.Menu.Categories.Components;
using MyHomeRamen.Blazor.Features.Menu.Common.Models;
using MyHomeRamen.Blazor.Features.Menu.Common.Services;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.Requests;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.Responses;
using MyHomeRamen.BlazorTests.Common.Helpers;

namespace MyHomeRamen.BlazorTests.MenuModule.Categories;

public sealed class CreateCategoryFormTests : BunitContext
{
    public CreateCategoryFormTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private void SetupMenuApiClient(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        HttpClient httpClient = new(new TestHttpMessageHandler(
            request => Task.FromResult(responseFactory(request))))
        {
            BaseAddress = new Uri("http://localhost"),
        };

        Services.AddSingleton(new MenuApiClient(httpClient));
    }

    private void SetupMenuApiClientAsync(Func<HttpRequestMessage, Task<HttpResponseMessage>> responseFactory)
    {
        HttpClient httpClient = new(new TestHttpMessageHandler(responseFactory))
        {
            BaseAddress = new Uri("http://localhost"),
        };

        Services.AddSingleton(new MenuApiClient(httpClient));
    }

    [Fact]
    public void Should_Render_NameInputAndSubmitButton()
    {
        // Arrange
        SetupMenuApiClient(_ => new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        IRenderedComponent<CreateCategoryForm> cut = Render<CreateCategoryForm>(p => p
            .Add(x => x.CategoryType, CategoryType.Product));

        // Assert
        cut.Find("input");
        cut.Find("button");
    }

    [Fact]
    public async Task Should_NotInvokeOnSuccess_When_NameIsEmpty()
    {
        // Arrange
        SetupMenuApiClient(_ => new HttpResponseMessage(HttpStatusCode.OK));

        bool successCalled = false;
        IRenderedComponent<CreateCategoryForm> cut = Render<CreateCategoryForm>(p => p
            .Add(x => x.CategoryType, CategoryType.Product)
            .Add(x => x.OnSuccess, EventCallback.Factory.Create<Guid>(this, _ => successCalled = true)));

        // Act – click submit without entering a name
        await cut.Find("button").ClickAsync(new MouseEventArgs());

        // Assert
        Assert.False(successCalled);
    }

    [Fact]
    public async Task Should_InvokeOnSuccess_When_SubmissionSucceeds()
    {
        // Arrange
        Guid expectedId = Guid.NewGuid();
        Guid? receivedId = null;

        SetupMenuApiClient(_ =>
        {
            HttpResponseMessage response = new(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new CreateCategoryResponse(expectedId)),
            };
            return response;
        });

        IRenderedComponent<CreateCategoryForm> cut = Render<CreateCategoryForm>(p => p
            .Add(x => x.CategoryType, CategoryType.Product)
            .Add(x => x.OnSuccess, EventCallback.Factory.Create<Guid>(this, id => receivedId = id)));

        // Act
        await cut.Find("input").ChangeAsync(new ChangeEventArgs { Value = "Noodles" });
        await cut.Find("button").ClickAsync(new MouseEventArgs());

        // Assert
        Assert.Equal(expectedId, receivedId);
    }

    [Fact]
    public async Task Should_ShowErrorAlert_When_ApiCallFails()
    {
        // Arrange
        SetupMenuApiClient(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));

        IRenderedComponent<CreateCategoryForm> cut = Render<CreateCategoryForm>(p => p
            .Add(x => x.CategoryType, CategoryType.Product));

        await cut.Find("input").ChangeAsync(new ChangeEventArgs { Value = "Noodles" });

        // Act
        await cut.Find("button").ClickAsync(new MouseEventArgs());

        // Assert
        cut.Find(".mud-alert");
        Assert.Contains("Failed to create category", cut.Markup);
    }

    [Fact]
    public async Task Should_SendCorrectCategoryType_In_ApiRequest()
    {
        // Arrange
        Guid expectedId = Guid.NewGuid();
        int? capturedCategoryType = null;

        SetupMenuApiClientAsync(async request =>
        {
            CreateCategoryRequest? body = await request.Content!.ReadFromJsonAsync<CreateCategoryRequest>();
            capturedCategoryType = body?.CategoryType;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new CreateCategoryResponse(expectedId)),
            };
        });

        IRenderedComponent<CreateCategoryForm> cut = Render<CreateCategoryForm>(p => p
            .Add(x => x.CategoryType, CategoryType.Ingredient));

        // Act
        await cut.Find("input").ChangeAsync(new ChangeEventArgs { Value = "Peppers" });
        await cut.Find("button").ClickAsync(new MouseEventArgs());

        // Assert
        Assert.Equal((int)CategoryType.Ingredient, capturedCategoryType);
    }
}
