using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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
    }
}
