using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Api.Common.Authorization;

namespace MyHomeRamen.Api.Common.Extentsions;

public static class HttpContextAccessorExtensions
{
    extension(IHttpContextAccessor httpContextAccessor)
    {
        public Guid GetGuidFromRouteParam(string key)
        {
            string value = (string)httpContextAccessor.HttpContext!.GetRouteValue(key)!;
            return Guid.Parse(value);
        }

        public string GetIdentityId()
        {
            return httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == ClaimConstants.KeycloakIdClaim)?.Value
                   ?? string.Empty;
        }

        public Guid? TryGetUserId()
        {
            Claim? domainIdClaim = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == ClaimConstants.DomainIdClaim);

            return Guid.TryParse(domainIdClaim?.Value, out Guid userId)
                 ? userId
                 : null;
        }

        public Guid? TryGetGuestId()
        {
            if (httpContextAccessor.HttpContext is null)
            {
                return null;
            }

            return
                httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("guest_id", out string? guestIdString)
                && Guid.TryParse(guestIdString, out Guid parsedId)
                ? parsedId
                : null;
        }
    }
}
