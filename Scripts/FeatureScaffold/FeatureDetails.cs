using System;
using System.Collections.Generic;
using System.Linq;

public record EndpointDetails(string Type, string? Route);

public record CommandDetails(string Type);

public record FeatureDetails(
    string Action,
    string Module,
    string Aggregate,
    string Name,
    EndpointDetails Endpoint,
    CommandDetails Command,
    IReadOnlyList<string> Constructors)
{
    public static FeatureDetails Create(string tableLine, string constructorsContent)
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
        string endpointType = endpointKind.Equals("query", StringComparison.OrdinalIgnoreCase) ? "Get" : "Post";
        EndpointDetails endpoint = new(endpointType, string.IsNullOrWhiteSpace(route) ? null : route);

        IReadOnlyList<string> constructors = ParseConstructors(constructorsContent);

        CommandDetails command = new(
            endpointKind.Equals("query", StringComparison.OrdinalIgnoreCase) ? "Query" : "Command");

        return new FeatureDetails(
            action,
            module,
            aggregate,
            featureName,
            endpoint,
            command,
            constructors);
    }

    private static IReadOnlyList<string> ParseConstructors(string constructorsContent)
    {
        if (string.IsNullOrWhiteSpace(constructorsContent))
        {
            return Array.Empty<string>();
        }

        List<string> constructors = [];
        string[] lines = constructorsContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            int declarationIndex = trimmed.IndexOf("public ", StringComparison.Ordinal);
            if (declarationIndex < 0 || !trimmed.Contains("record ", StringComparison.Ordinal))
            {
                continue;
            }

            string declaration = trimmed[declarationIndex..];
            constructors.Add(declaration);
        }

        return constructors;
    }

}
