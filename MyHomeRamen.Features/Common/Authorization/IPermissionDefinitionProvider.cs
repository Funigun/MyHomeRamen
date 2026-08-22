namespace MyHomeRamen.Features.Common.Authorization;

public interface IPermissionDefinitionProvider
{
    string ModuleName { get; }

    IReadOnlyCollection<PermissionDefinition> Permissions { get; }

    IReadOnlyCollection<PermissionDefinition> GuestPermissions { get; }
}
