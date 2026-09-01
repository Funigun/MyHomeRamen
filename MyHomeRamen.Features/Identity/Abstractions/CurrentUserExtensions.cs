using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Identity.Abstractions;

internal static class CurrentUserExtensions
{
    extension(ICurrentUser currentUser)
    {
        internal bool CanViewUserProfile() => currentUser.Permissions.Contains($"{PermissionConstants.CanViewUserProfile}");

        internal bool CanEditUserProfile() => CanViewUserProfile(currentUser) 
                                           && currentUser.Permissions.Contains($"{PermissionConstants.CanUpdateUserProfile}");

        internal bool CanDeleteUserProfile() => CanViewUserProfile(currentUser) 
                                             && currentUser.Permissions.Contains($"{PermissionConstants.CanDeleteUserProfile}");
    }
}
