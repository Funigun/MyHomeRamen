# Feature Implementation Plan — Frontend

- **Date**: 2025-07-18
- **Feature**: Home Page Blazor Refactor — Convert HTML prototype to configurable Blazor components
- **Module**: Home (new feature area under `Features/Home/`)
- **Scope**: Frontend only (backend out of scope)
- **Reference HTML**: `.github/agents/input/blazor-page.md`

---

## Overview

Convert the static HTML home page prototype (`.github/agents/input/blazor-page.md`) into a fully componentized Blazor page using MudBlazor. The design must be **theme-configurable** via `ThemeConfiguration` / `appsettings.json` so different restaurants can adjust branding (colors, fonts, restaurant name) without code changes.

### Key Design Decisions

1. **Two AppBars** — `CustomerAppBar` and `EmployeeAppBar` replace the current inline `<MudAppBar>` blocks in `CustomerLayout.razor` and `EmployeeLayout.razor`. The Home page itself does NOT include an AppBar — it is rendered by the layout.
2. **Shared reusable components** — Footer, HeroSection, ImageDisplay, MenuItemCard, etc. are layout/role-agnostic and shared across Customer and Employee contexts.
3. **ImageDisplay component** — A reusable component that accepts an image model (URL + alt text) and renders the image or a fallback placeholder icon.
4. **Extended ThemeConfiguration** — `PaletteConfiguration` gains additional color tokens mapped from the HTML prototype (surface variants, tertiary, outline, error, etc.) to support the rich design system.

---

## Phase 1: Configuration & Theme Foundation

### 1.1) Extend `ThemeConfiguration` and `PaletteConfiguration`

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Common/Configuration/ThemeConfiguration.cs`

Extend `PaletteConfiguration` with additional MudBlazor `Palette` properties needed to map the HTML design tokens:

```
PaletteConfiguration
??? Primary              (existing)
??? Secondary            (existing)
??? AppbarBackground     (existing)
??? Tertiary             (new)
??? Background           (new)
??? Surface              (new)
??? TextPrimary          (new — maps on-surface)
??? TextSecondary        (new — maps on-surface-variant)
??? DrawerBackground     (new)
??? DrawerText           (new)
??? Error                (new)
??? AppbarText           (new)
```

Add a new `TypographyConfiguration` class for font customization:

```
ThemeConfiguration
??? PaletteLight         (existing, extended)
??? PaletteDark          (existing, extended)
??? Typography           (new)
?   ??? HeadlineFont     (default: "Newsreader, serif")
?   ??? BodyFont         (default: "Be Vietnam Pro, sans-serif")
?   ??? LabelFont        (default: "Be Vietnam Pro, sans-serif")
```

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Common/Configuration/ThemeProviderService.cs`

Update `MapPalette<T>()` to map all new properties to `MudBlazor.Palette` fields.
Update `GetTheme()` to also configure `MudTheme.Typography` using the new `TypographyConfiguration`.

### 1.2) Extend `RestaurantConfiguration`

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Common/Configuration/RestaurantConfiguration.cs`

Add new configurable properties sourced from `appsettings.json` under `RestaurantConfiguration`:

```
RestaurantConfiguration
??? RestaurantName       (existing)
??? RestaurantId         (existing)
??? InfrastructurePrefix (existing)
??? LayoutType           (existing)
??? Tagline              (new — e.g., "Artisan Craft, Hearth-Born.")
??? Description          (new — hero subtitle text)
??? EstablishedYear      (new — e.g., "1982")
??? Location             (new)
?   ??? Address          (new — e.g., "Shimogyo Ward, Kyoto, 600-8216, Japan")
?   ??? MapImageUrl      (new — optional)
??? SeasonLabel          (new — e.g., "Autumn Collection")
??? Copyright            (new — e.g., "© 2024 THE HEARTHSIDE MANUSCRIPT. ALL RIGHTS RESERVED.")
```

### 1.3) Update `appsettings.json`

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/appsettings.json`

Add the extended theme tokens and restaurant configuration matching the HTML prototype's color palette:

```json
{
  "ThemeConfiguration": {
    "PaletteLight": {
      "Primary": "#6c2f00",
      "Secondary": "#725a39",
      "Tertiary": "#45422e",
      "AppbarBackground": "#fff8f6",
      "AppbarText": "#6c2f00",
      "Background": "#fff8f6",
      "Surface": "#fff8f6",
      "TextPrimary": "#2b1613",
      "TextSecondary": "#54433a",
      "DrawerBackground": "#2b1613",
      "DrawerText": "#ffe9e5",
      "Error": "#ba1a1a"
    },
    "PaletteDark": {
      "Primary": "#ffb68c",
      "Secondary": "#e1c299",
      "Tertiary": "#cdc7ad",
      "AppbarBackground": "#1a110f",
      "AppbarText": "#ffb591",
      "Background": "#1a110f",
      "Surface": "#1a110f",
      "TextPrimary": "#ffe9e5",
      "TextSecondary": "#dac2b6",
      "DrawerBackground": "#120807",
      "DrawerText": "#ffe9e5",
      "Error": "#ffb4ab"
    },
    "Typography": {
      "HeadlineFont": "Newsreader, serif",
      "BodyFont": "Be Vietnam Pro, sans-serif",
      "LabelFont": "Be Vietnam Pro, sans-serif"
    }
  },
  "RestaurantConfiguration": {
    "Tagline": "Artisan Craft, Hearth-Born.",
    "Description": "Step inside the Machiya of your dreams...",
    "EstablishedYear": "1982",
    "Location": {
      "Address": "Shimogyo Ward, Kyoto, 600-8216, Japan",
      "MapImageUrl": ""
    },
    "SeasonLabel": "Autumn Collection",
    "Copyright": "© 2024 THE HEARTHSIDE MANUSCRIPT. ALL RIGHTS RESERVED."
  }
}
```

### 1.4) Update `App.razor` — Add fonts

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Components/App.razor`

Add Google Fonts link for `Newsreader` and `Be Vietnam Pro` (from the HTML prototype) alongside existing Roboto:
```html
<link href="https://fonts.googleapis.com/css2?family=Newsreader:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400&family=Be+Vietnam+Pro:wght@100;300;400;500;700;900&display=swap" rel="stylesheet" />
```

---

## Phase 2: AppBar Components (Customer & Employee)

### 2.1) Create `CustomerAppBar.razor`

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Components/Layout/Customer/CustomerAppBar.razor`

Extract the AppBar from `CustomerLayout.razor` into a dedicated component. Map the HTML prototype's customer navigation:

```
CustomerAppBar
??? Restaurant name (from RestaurantConfiguration.RestaurantName)
??? Navigation links: Menu, Orders, Gallery, Story (MudNavLinks/MudButtons)
??? Right-side actions: Shopping Cart (icon), Settings (icon), Login component
??? Styled using theme colors (AppbarBackground, AppbarText from palette)
```

- Use `MudAppBar` with `MudToolBar` inside
- Use `MudIconButton` with `Icon` for cart/settings
- Embed existing `<Login />` component
- Navigation uses `href` attributes (not `NavigationManager` directly per instructions)

### 2.2) Create `EmployeeAppBar.razor`

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Components/Layout/Employee/EmployeeAppBar.razor`

Extract the AppBar from `EmployeeLayout.razor` into a dedicated component:

```
EmployeeAppBar
??? Restaurant name + " - Employee" suffix
??? Navigation links: Menu, Products, Orders, Reservations, Tables
??? Right-side actions: Account, Login
```

### 2.3) Update `CustomerLayout.razor`

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Components/Layout/Customer/CustomerLayout.razor`

Replace the inline `<MudAppBar>` block with `<CustomerAppBar />`.
Keep `MudThemeProvider`, `MudPopoverProvider`, `MudDialogProvider`, `MudSnackbarProvider`, and `MudLayout` / `MudMainContent` wrappers.

### 2.4) Update `EmployeeLayout.razor`

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Components/Layout/Employee/EmployeeLayout.razor`

Replace the inline `<MudAppBar>` block with `<EmployeeAppBar />`.

---

## Phase 3: Shared Reusable Components

### 3.1) Image Model & ImageDisplay Component

**Model file**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Common/Models/ImageModel.cs`

```csharp
public sealed class ImageModel
{
    public string? Url { get; init; }
    public string Alt { get; init; } = string.Empty;
}
```

**Component file**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Components/Common/ImageDisplay.razor`

Parameters:
```
[Parameter] public ImageModel? Image { get; set; }
[Parameter] public string? Class { get; set; }
[Parameter] public string? Style { get; set; }
[Parameter] public string FallbackIcon { get; set; } = Icons.Material.Filled.Image;
```

Behavior:
- If `Image?.Url` is non-empty ? render `<MudImage>` with the URL and alt text
- If `Image` is null or URL is empty ? render a `<MudPaper>` with centered `<MudIcon>` as placeholder
- Apply `Class` and `Style` to the outer container for flexible sizing

### 3.2) Footer Component

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Components/Common/Footer.razor`

A shared footer component used by both Customer and Employee layouts. Maps the HTML prototype's footer:

```
Footer
??? Restaurant name (from RestaurantConfiguration)
??? Copyright text (from RestaurantConfiguration.Copyright)
??? Navigation links: Privacy Policy, Terms of Service, Press Kit, Contact
??? Social icons (MudIconButtons)
??? Dark background using theme's inverse surface colors
```

Parameters:
```
(no parameters — injects RestaurantConfiguration directly)
```

**Scoped CSS file**: `Footer.razor.css` — dark background styling, link colors

After creating the Footer component, update both `CustomerLayout.razor` and `EmployeeLayout.razor` to include `<Footer />` after `<MudMainContent>` but still inside `<MudLayout>`.

### 3.3) MenuItemCard Component

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Home/Components/MenuItemCard.razor`

A reusable card for displaying a menu item on the home page:

```
Parameters:
??? [Parameter] public string Title { get; set; }
??? [Parameter] public string Description { get; set; }
??? [Parameter] public string Price { get; set; }
??? [Parameter] public ImageModel? Image { get; set; }
??? [Parameter] public string? Badge { get; set; }            — optional badge text (e.g., "Chef's Signature")
??? [Parameter] public bool UseAltBackground { get; set; }    — switches to tertiary-styled background
??? [Parameter] public int ColSpan { get; set; } = 1           — for grid layout (1 or 2)
```

Renders a `<MudCard>` with:
- `<ImageDisplay>` for the item image
- Title, description, price typography
- Optional badge overlay

### 3.4) StatHighlight Component

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Home/Components/StatHighlight.razor`

Small reusable component for the "36 Hours / Broth Simmering Time" style stats:

```
Parameters:
??? [Parameter] public string Value { get; set; }
??? [Parameter] public string Label { get; set; }
```

Renders with a left border accent and stacked value/label typography.

### 3.5) Home Page Data Model

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Home/Models/HomePageModel.cs`

Defines the data model for the configurable home page sections:

```csharp
public sealed class HomePageModel
{
    public HeroSectionModel Hero { get; init; } = new();
    public StorySectionModel Story { get; init; } = new();
    public MenuHighlightsSectionModel MenuHighlights { get; init; } = new();
    public SeasonalHighlightSectionModel SeasonalHighlight { get; init; } = new();
    public AtmosphereSectionModel Atmosphere { get; init; } = new();
}

public sealed class HeroSectionModel
{
    public string Subtitle { get; init; } = string.Empty;      // "The Spirit of My Home Ramen"
    public string HeadlineLine1 { get; init; } = string.Empty;  // "Artisan Craft,"
    public string HeadlineLine2 { get; init; } = string.Empty;  // "Hearth-Born."
    public string Description { get; init; } = string.Empty;
    public ImageModel? BackgroundImage { get; init; }
    public string PrimaryCtaText { get; init; } = string.Empty;   // "Reserve a Seat"
    public string PrimaryCtaHref { get; init; } = string.Empty;
    public string SecondaryCtaText { get; init; } = string.Empty;  // "Explore Menu"
    public string SecondaryCtaHref { get; init; } = string.Empty;
}

public sealed class StorySectionModel
{
    public ImageModel? Image { get; init; }
    public string Title { get; init; } = string.Empty;
    public List<string> Paragraphs { get; init; } = [];
    public List<StatHighlightModel> Stats { get; init; } = [];
}

public sealed class StatHighlightModel
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
}

public sealed class MenuHighlightsSectionModel
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public List<MenuItemModel> Items { get; init; } = [];
}

public sealed class MenuItemModel
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Price { get; init; } = string.Empty;
    public ImageModel? Image { get; init; }
    public string? Badge { get; init; }
    public bool UseAltBackground { get; init; }
    public int ColSpan { get; init; } = 1;
}

public sealed class SeasonalHighlightSectionModel
{
    public string Label { get; init; } = string.Empty;          // "Now Serving"
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string LinkText { get; init; } = string.Empty;
    public string LinkHref { get; init; } = string.Empty;
    public ImageModel? Image { get; init; }
}

public sealed class AtmosphereSectionModel
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ImageModel? AtmosphereImage { get; init; }
}
```

This model can later be populated from an API or config. For the initial implementation, it will be hardcoded with values from the HTML prototype.

---

## Phase 4: Home Page Sections (as Blazor Components)

Each major HTML section becomes a standalone Blazor component under `Features/Home/Components/`.

### 4.1) HeroSection Component

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Home/Components/HeroSection.razor`

**Scoped CSS**: `HeroSection.razor.css` — gradient overlays, background image positioning

Parameters:
```
[Parameter] public HeroSectionModel Model { get; set; }
```

Maps the HTML hero section:
- Full-width background image with gradient overlay (CSS)
- Headline with mixed italic/bold typography
- Subtitle label
- Description paragraph
- Two CTA buttons (primary gradient via `MudButton` + secondary outlined)

### 4.2) StorySection Component

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Home/Components/StorySection.razor`

**Scoped CSS**: `StorySection.razor.css` — rotated image frame, badge positioning

Parameters:
```
[Parameter] public StorySectionModel Model { get; set; }
```

Maps "The Manuscript" section:
- `MudGrid` 12-column: image (5 cols via `MudItem xs="12" md="5"`) + text content (7 cols)
- Rotated image frame with rounded badge ("Est. 1982" from `RestaurantConfiguration.EstablishedYear`)
- Story text paragraphs
- `<StatHighlight>` components for stats (e.g., "36 Hours", "Daily")

### 4.3) MenuHighlightsSection Component

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Home/Components/MenuHighlightsSection.razor`

Parameters:
```
[Parameter] public MenuHighlightsSectionModel Model { get; set; }
```

Maps "The Daily Selection" / Bento Menu Grid:
- Section header with title + season label (from `RestaurantConfiguration.SeasonLabel`)
- `MudGrid` with `<MenuItemCard>` components
- Items: Tantanmen (2-col span, badge), Okonomiyaki (1 col, alt bg), Sushi (1 col, tertiary bg)

### 4.4) SeasonalHighlightSection Component

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Home/Components/SeasonalHighlightSection.razor`

Parameters:
```
[Parameter] public SeasonalHighlightSectionModel Model { get; set; }
```

Maps "The Chef's Highlight" / seasonal feature:
- Two-column `MudGrid` layout: text + framed image
- "Now Serving" label
- Feature name, description, "Discover the story" `MudLink`

### 4.5) LocationSection Component

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Home/Components/LocationSection.razor`

**Scoped CSS**: `LocationSection.razor.css` — floating overlay card

Parameters:
```
[Parameter] public AtmosphereSectionModel Model { get; set; }
```

Maps the Location/Atmosphere section:
- Left column: address card (`MudPaper`) with map `<ImageDisplay>` (from `RestaurantConfiguration.Location`)
- Right column: full-height atmosphere `<ImageDisplay>` with floating `<MudPaper>` text overlay card

---

## Phase 5: Home Page Assembly

### 5.1) Create Home Page

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Home/HomePage.razor`

```razor
@page "/"
@page "/home"

@attribute [AllowAnonymous]

@inject RestaurantConfiguration RestaurantConfiguration

<PageTitle>@RestaurantConfiguration.RestaurantName — Home</PageTitle>

<HeroSection Model="_model.Hero" />
<StorySection Model="_model.Story" />
<MenuHighlightsSection Model="_model.MenuHighlights" />
<SeasonalHighlightSection Model="_model.SeasonalHighlight" />
<LocationSection Model="_model.Atmosphere" />
```

The page is thin — it composes the section components with a data model.
Initially the `HomePageModel` is populated with hardcoded prototype data in `@code`.
Later this can be driven by an API or configuration.

### 5.2) Register Home Navigation

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Home/Common/Services/HomeNavigationService.cs`

```csharp
public sealed class HomeNavigationService(NavigationManager navigation)
{
    public static class Routes
    {
        public const string Home = "/home";
    }

    public void NavigateToHome() => navigation.NavigateTo(Routes.Home);
}
```

**Update**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Presentation/NavigationDependencyInjection.cs`

Register `HomeNavigationService`.

---

## Final Folder Tree

```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/
??? Common/
?   ??? Configuration/
?   ?   ??? ThemeConfiguration.cs           ? UPDATED (extended PaletteConfiguration + TypographyConfiguration)
?   ?   ??? ThemeProviderService.cs         ? UPDATED (map new palette/typography fields)
?   ?   ??? RestaurantConfiguration.cs      ? UPDATED (new properties: Tagline, Location, etc.)
?   ?   ??? LayoutType.cs                   (unchanged)
?   ??? Models/
?       ??? FormMode.cs                     (unchanged)
?       ??? BaseValidator.cs                (unchanged)
?       ??? ImageModel.cs                   ? NEW
??? Components/
?   ??? Common/
?   ?   ??? ImageDisplay.razor              ? NEW
?   ?   ??? Footer.razor                    ? NEW
?   ?   ??? Footer.razor.css               ? NEW
?   ??? Layout/
?   ?   ??? Customer/
?   ?   ?   ??? CustomerAppBar.razor        ? NEW
?   ?   ?   ??? CustomerLayout.razor        ? UPDATED (use CustomerAppBar + Footer)
?   ?   ?   ??? CustomerLayout.razor.css    (unchanged)
?   ?   ??? Employee/
?   ?   ?   ??? EmployeeAppBar.razor        ? NEW
?   ?   ?   ??? EmployeeLayout.razor        ? UPDATED (use EmployeeAppBar + Footer)
?   ?   ?   ??? EmployeeLayout.razor.css    (unchanged)
?   ?   ??? Login.razor                     (unchanged)
?   ?   ??? ReconnectModal.razor            (unchanged)
?   ??? App.razor                           ? UPDATED (add font links)
?   ??? Routes.razor                        (unchanged)
??? Features/
?   ??? Home/
?       ??? Common/
?       ?   ??? Services/
?       ?       ??? HomeNavigationService.cs      ? NEW
?       ??? Components/
?       ?   ??? HeroSection.razor                 ? NEW
?       ?   ??? HeroSection.razor.css             ? NEW
?       ?   ??? StorySection.razor                ? NEW
?       ?   ??? StorySection.razor.css            ? NEW
?       ?   ??? MenuHighlightsSection.razor       ? NEW
?       ?   ??? MenuItemCard.razor                ? NEW
?       ?   ??? SeasonalHighlightSection.razor    ? NEW
?       ?   ??? LocationSection.razor             ? NEW
?       ?   ??? LocationSection.razor.css         ? NEW
?       ?   ??? StatHighlight.razor               ? NEW
?       ??? Models/
?       ?   ??? HomePageModel.cs                  ? NEW (includes all section sub-models)
?       ??? HomePage.razor                        ? NEW (@page "/" and "/home")
??? Presentation/
?   ??? NavigationDependencyInjection.cs    ? UPDATED (register HomeNavigationService)
??? appsettings.json                        ? UPDATED (extended theme + restaurant config)
```

---

## Implementation Order

Following the user's suggested approach:

| Step | What | Files | Dependencies |
|---|---|---|---|
| **1** | Extend configuration models | `ThemeConfiguration.cs`, `RestaurantConfiguration.cs`, `appsettings.json` | None |
| **2** | Update `ThemeProviderService.cs` | `ThemeProviderService.cs` | Step 1 |
| **3** | Add font links | `App.razor` | None |
| **4** | Create `ImageModel` + `ImageDisplay` | `ImageModel.cs`, `ImageDisplay.razor` | None |
| **5** | Create `CustomerAppBar.razor` | `CustomerAppBar.razor` | Step 1 |
| **6** | Create `EmployeeAppBar.razor` | `EmployeeAppBar.razor` | Step 1 |
| **7** | Update `CustomerLayout.razor` | `CustomerLayout.razor` | Step 5 |
| **8** | Update `EmployeeLayout.razor` | `EmployeeLayout.razor` | Step 6 |
| **9** | Create `Footer.razor` + `Footer.razor.css` | `Footer.razor`, `Footer.razor.css` | Step 1 |
| **10** | Add `<Footer />` to both layouts | `CustomerLayout.razor`, `EmployeeLayout.razor` | Step 9 |
| **11** | Create Home page models | `HomePageModel.cs` | Step 4 |
| **12** | Create `StatHighlight.razor` | `StatHighlight.razor` | None |
| **13** | Create `MenuItemCard.razor` | `MenuItemCard.razor` | Step 4 |
| **14** | Create `HeroSection.razor` + CSS | `HeroSection.razor`, `HeroSection.razor.css` | Step 4, 11 |
| **15** | Create `StorySection.razor` + CSS | `StorySection.razor`, `StorySection.razor.css` | Step 4, 11, 12 |
| **16** | Create `MenuHighlightsSection.razor` | `MenuHighlightsSection.razor` | Step 11, 13 |
| **17** | Create `SeasonalHighlightSection.razor` | `SeasonalHighlightSection.razor` | Step 4, 11 |
| **18** | Create `LocationSection.razor` + CSS | `LocationSection.razor`, `LocationSection.razor.css` | Step 4, 11 |
| **19** | Compose `HomePage.razor` | `HomePage.razor` | Steps 14–18 |
| **20** | Create `HomeNavigationService` + register | `HomeNavigationService.cs`, `NavigationDependencyInjection.cs` | None |

---

## 16) Unit tests for Blazor components and services

> Unit tests for Blazor components are **deferred** — the `blazor-tests.instructions.md` is currently marked as `TODO`.
> Once the testing framework for Blazor is established, the following test coverage should be added:
>
> - `ImageDisplay` renders `<MudImage>` when URL is provided
> - `ImageDisplay` renders fallback icon when URL is null/empty
> - `Footer` renders restaurant name and copyright from configuration
> - `CustomerAppBar` renders expected navigation links
> - `EmployeeAppBar` renders expected navigation links
> - `MenuItemCard` renders title, price, description, optional badge
> - `StatHighlight` renders value and label
> - `HomePage` composes all sections

---

## Notes & Risks

1. **MudBlazor CSS vs Tailwind**: The HTML prototype uses Tailwind CSS classes extensively. In Blazor, we use MudBlazor components + scoped CSS (`.razor.css`) for custom styling. Do NOT add Tailwind to the Blazor project. Translate Tailwind utilities to either MudBlazor `Class` parameters or scoped CSS.

2. **Image URLs**: The prototype uses Google-hosted placeholder images. For the initial implementation, keep these URLs in the hardcoded `HomePageModel`. Eventually these will come from an API/CMS.

3. **Responsive design**: MudBlazor provides `MudGrid`, `MudItem`, and `MudHidden` for responsive layouts. Use these instead of Tailwind's responsive prefixes (`md:`, `lg:`).

4. **Dark mode**: The prototype defines a dark color scheme. MudBlazor's `MudThemeProvider` supports dark mode toggle via `IsDarkMode`. The extended `PaletteDark` configuration already supports this — a dark mode toggle can be added to the AppBars as a future enhancement.

5. **Scoped CSS files**: Each new component that requires custom styling beyond MudBlazor defaults should have a companion `.razor.css` file with scoped styles. Particularly: `HeroSection.razor.css` (gradient overlays, background image), `StorySection.razor.css` (rotated image frame, badge), `Footer.razor.css` (dark background styling), `LocationSection.razor.css` (floating overlay card).
