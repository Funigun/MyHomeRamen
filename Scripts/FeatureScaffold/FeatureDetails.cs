using System;
using System.Collections.Generic;
using System.Linq;

public record EndpointDetails(string Type, string? Route);

public record CommandDetails(string Type);

public record FeatureDetails(string Action, string Module,string Aggregate, string Name, EndpointDetails Endpoint, CommandDetails Command)
{
    public static FeatureDetails Create(string tableLine)
    {
        string[] parts = tableLine
            .Split('|')
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .ToArray();

        if (parts.Length < 6)
        {
            throw new ArgumentException($"Invalid feature row: {tableLine}", nameof(tableLine));
        }

        string action = parts[0].ToLowerInvariant();
        string module = parts[1];
        string aggregate = parts[2];
        string featureName = parts[3];
        string endpointKind = parts[4];
        string route = parts[5];
        string endpointType = CalculateEndpointType(route, endpointKind);
        EndpointDetails endpoint = new(endpointType, string.IsNullOrWhiteSpace(route) ? null : CalculateEndpointRoute(route, endpointKind));

        CommandDetails command = new(CalculateCommandType(endpointKind));

        return new FeatureDetails(
            action,
            module,
            aggregate,
            featureName,
            endpoint,
            command);
    }

    private static string CalculateEndpointType(string route, string endpointKind) => endpointKind == "Query" || endpointKind == "Command" ? route.Split(":")[0].Trim() : string.Empty;
    private static string CalculateEndpointRoute(string route, string endpointKind) => endpointKind == "Query" || endpointKind == "Command" ? route.Split(":")[1].Trim() : string.Empty;

    private static string CalculateCommandType(string cqrsType) => cqrsType.ToLowerInvariant() switch
    {

       "query" => "Query",
        "command" => "Command",
        "integrationtest" => "IntegrationTest",
        _ => throw new ArgumentException($"Invalid CQRS type: {cqrsType}", nameof(cqrsType)),
    };

    public bool IsQuery => Command.Type.Equals("Query", StringComparison.OrdinalIgnoreCase);

    public bool ScaffoldIntegrationTest => Command.Type.Equals("IntegrationTest", StringComparison.OrdinalIgnoreCase);

    public bool RequireResponse => Endpoint.Type == "Get" || Endpoint.Type == "Post";
}
