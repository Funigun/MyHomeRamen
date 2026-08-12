using System;
using System.Collections.Generic;
using System.Linq;

namespace MyHomeRamen.FeatureScaffold;

public record EndpointDetails(string Type, string? Route);

public record CommandDetails(string Type, bool HasDbQueryOptions, bool HasValidator, bool hasAuthPolicy);

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

        if (parts.Length < 8)
        {
            throw new ArgumentException($"Invalid feature row: {tableLine}", nameof(tableLine));
        }

        string action = parts[1];
        string module = parts[2];
        string aggregate = parts[3];
        string featureName = parts[4];
        string endpointKind = parts[5];
        string route = parts[6];
        string dbQueryOptionsRequired = parts[7];
        HashSet<string> policies = parts.Length > 8 ? new HashSet<string>(parts[8].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim())) : new HashSet<string>();

        EndpointDetails endpoint = new(endpointKind,string.IsNullOrWhiteSpace(route) ? null : route);

        IReadOnlyList<string> constructors = ParseConstructors(constructorsContent, featureName);

        CommandDetails command = new(
            endpointKind == "query" ? "Query" : "Command",
            ParseBoolean(dbQueryOptionsRequired),
            policies.Contains("ValidationPolicy"),
            policies.Contains("AuthPolicy"));

        return new FeatureDetails(
            action,
            module,
            aggregate,
            featureName,
            endpoint,
            command,
            constructors);
    }

    private static IReadOnlyList<string> ParseConstructors(string constructorsContent, string featureName)
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
            if (string.IsNullOrWhiteSpace(trimmed) || !trimmed.Contains(featureName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            int openBracketIndex = trimmed.IndexOf('(');
            int closeBracketIndex = trimmed.LastIndexOf(')');

            if (openBracketIndex < 0 || closeBracketIndex <= openBracketIndex)
            {
                continue;
            }

            string signature = trimmed.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            
            if (string.IsNullOrWhiteSpace(signature))
            {
                continue;
            }

            string[] parameters = signature.Split(',')
                                           .Select(p => p.Trim())
                                           .Where(p => !string.IsNullOrWhiteSpace(p))
                                           .ToArray();

            foreach (string parameter in parameters)
            {
                string[] parts = parameter.Split(':', 2, StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                {
                    constructors.Add(parts[0].Trim());
                }
                else
                {
                    constructors.Add(parameter.Trim());
                }
            }
        }

        return constructors;
    }

    private static bool ParseBoolean(string value)
        => bool.TryParse(value, out bool parsed) && parsed;
}
