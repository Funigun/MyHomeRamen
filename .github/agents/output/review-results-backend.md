- **Date**: 2026-04-04 00:14:34 +02:00
- **Feature**: GetCategoriesByType
- **Critical**: 0
- **Warnings**: 1
- **Information**: 1

---

- **Title**: [1] [MyHomeRamen.IntegrationTests/MenuModule/GetCategoriesByTypeTests.cs : 14] - Missing invalid `categoryType` regression coverage
- **Severity**: Warning
- **Description**: The refactor keeps `GetCategoriesByTypeValidator` and still accepts an untrusted `int` query parameter, but the new integration suite no longer verifies that invalid enum values return `400 Bad Request`. The removed `GetCategoriesForDropdown` tests covered this behavior, so the refactor reduces regression protection around request validation.
- **Solution proposal**: Add a theory covering invalid values such as `0`, `-1`, and `999`, call `/api/menu/categories/by-type`, and assert `HttpStatusCode.BadRequest` using the same request setup pattern already used in this file.

- **Title**: [2] [MyHomeRamen.Api/MyHomeRamen.Api.csproj : 17] - SDK project still contains folder entries for deleted category features
- **Severity**: Information
- **Description**: The API project file still declares `Folder Include` entries for `GetCategoriesForDropdown` and `GetCategoriesForManage` even though those feature folders were removed. This does not break the build, but it leaves stale project metadata behind and makes the project structure drift from the actual source layout.
- **Solution proposal**: Remove the obsolete `Folder Include` entries for `Menu\Features\Categories\GetCategoriesForDropdown\Models\`, `Menu\Features\Categories\GetCategoriesForDropdown\Policies\`, and `Menu\Features\Categories\GetCategoriesForManage\Models\` from `MyHomeRamen.Api.csproj`.

## Validation summary

- `dotnet build` ✅
- `dotnet test MyHomeRamen.ArchitectureTests/MyHomeRamen.ArchitectureTests.csproj --no-build` ✅
- `dotnet test MyHomeRamen.UnitTests/MyHomeRamen.UnitTests.csproj --no-build` ✅
- `dotnet test MyHomeRamen.IntegrationTests/MyHomeRamen.IntegrationTests.csproj --no-build` ✅
