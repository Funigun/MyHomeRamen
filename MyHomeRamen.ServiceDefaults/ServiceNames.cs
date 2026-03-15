namespace MyHomeRamen.ServiceDefaults;

public static class ServiceNames
{
    // Application services
    public static string Api(string prefix) => $"{prefix}-api";

    public static string IdentityApi(string prefix) => $"{prefix}-identity-api";

    public static string Blazor(string prefix) => $"{prefix}-blazor";

    // Infrastructure resources
    public static string Cache(string prefix) => $"{prefix}-cache";

    public static string RabbitMq(string prefix) => $"{prefix}-rabbitmq";

    public static string KeyCloak(string prefix) => $"{prefix}-key-cloak";

    // Workers
    public static string DbInitializerWorker(string prefix) => $"{prefix}-db-initializer";

    public static string MessagesWorker(string prefix) => $"{prefix}-messages-worker";

    public static string MailingWorker(string prefix) => $"{prefix}-mailing-worker";
}
