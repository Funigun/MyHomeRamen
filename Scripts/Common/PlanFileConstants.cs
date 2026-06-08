public static class PlanFileConstants
{
    public const string ProblemSectionHeader = "## 1. Problem";
    public const string FilesSectionHeader = "## 2. Files to create / modify";
    public const string DomainChangesSectionHeader = "## 3. Domain changes";
    public const string PersistenceExtensionsSectionHeader = "## 4. Persistence extensions";
    public const string ApiDetailsSectionHeader = "## 5. API details";
    public const string TestsSectionHeader = "## 6. Tests";
    public const string RisksSectionHeader = "## 7. Risks / decisions for human approval";
    public const string OutOfScopeSectionHeader = "## 8. Out of scope";

    public static readonly HashSet<string> ExpectedSectionHeaders = new()
    {
        ProblemSectionHeader,
        FilesSectionHeader,
        DomainChangesSectionHeader,
        PersistenceExtensionsSectionHeader,
        ApiDetailsSectionHeader,
        TestsSectionHeader,
        RisksSectionHeader,
        OutOfScopeSectionHeader
    };
}
