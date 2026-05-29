public enum FileType
{
    Unknown,
    Request,
    Response,
    Command,
    CommandHandler,
    CommandVoid,
    CommandVoidHandler,
    Query,
    QueryHandler,
    Validator,
    GetEndpoint,
    PostEndpoint,
    PutEndpoint,
    DeleteEndpoint,
    UnitTest,
    IntegrationTest
}

public static class FileTypeExtensions
{
    public static FileType ToFileType(this string typeString)
    {
        return typeString.Trim().ToLower() switch
        {
            "request" => FileType.Request,
            "response" => FileType.Response,
            "command" => FileType.Command,
            "command-handler" => FileType.CommandHandler,
            "command-void" => FileType.CommandVoid,
            "command-void-handler" => FileType.CommandVoidHandler,
            "query" => FileType.Query,
            "query-handler" => FileType.QueryHandler,
            "validator" => FileType.Validator,
            "endpoint-get" => FileType.GetEndpoint,
            "endpoint-post" => FileType.PostEndpoint,
            "endpoint-put" => FileType.PutEndpoint,
            "endpoint-delete" => FileType.DeleteEndpoint,
            "unit-test" => FileType.UnitTest,
            "integration-test" => FileType.IntegrationTest,
            _ => FileType.Unknown
        };
    }
}
