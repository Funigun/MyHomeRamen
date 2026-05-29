public enum FileAction
{
    Create,
    Modify,
    Delete
}

public static class FileActionExtensions
{
    public static FileAction ToFileAction(this string actionString)
    {
        return actionString.Trim().ToLower() switch
        {
            "create" => FileAction.Create,
            "modify" => FileAction.Modify,
            "delete" => FileAction.Delete,
            _ => throw new ArgumentException($"Invalid action: {actionString}")
        };
    }
}
