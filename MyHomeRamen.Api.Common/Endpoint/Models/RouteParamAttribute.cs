namespace MyHomeRamen.Api.Common.Endpoint.Models;

/// <summary>
/// Marks a property on a request record as route-bound.
/// Route parameters are excluded from Blazor/API contract sync checks,
/// since they are supplied by the URL rather than the request body.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class RouteParamAttribute : Attribute;
