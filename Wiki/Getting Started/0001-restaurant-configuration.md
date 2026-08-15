# Restaurant Configuration

The `MyHomeRamen` application uses a centralized configuration approach to manage different restaurant instances. 
This configuration is primarily handled in `MyHomeRamen.AppHost` and allows for setting up the environment for a specific restaurant, 
including its infrastructure and identity settings.

## AppHost Orchestration

The `AppHost.cs` file is the entry point for the Aspire orchestration. It reads the configuration and sets up the distributed application, orchestrating the following resources:
- Infrastructure: Redis Cache, RabbitMQ, Keycloak.
- Projects: API, Identity API, Blazor Frontend, Database Initializer.
- Workers: Mail Sender, Messages Handler.

## Configuration Section

The `RestaurantConfiguration` section in `appsettings.Development.json` is crucial for identifying the restaurant and properly naming infrastructure resources.

```json
"RestaurantConfiguration": {
  "Name": "My Home Ramen",
  "InfrastructurePrefix": "my-home-ramen"
}
```

### Fields

#### Name
- **Description**: The display name of the restaurant.
- **Usage**: Used primarily on the frontend application (`MyHomeRamen.Blazor`) to display the restaurant name to the user.

#### InfrastructurePrefix
- **Description**: A string prefix used for naming infrastructure resources and services.
- **Usage**: 
    - **Aspire Orchestrator**: In `RegistrationExtensions.cs` and `ProjectRegistrationExtensions.cs`, this prefix is used to construct resource names (e.g., `{prefix}-redis`, `{prefix}-api`, `{prefix}-rabbitmq`).
    - **Service Discovery**: Concrete services rely on these prefixed names to discover and connect to required dependencies (e.g., the API project referring to the Redis cache).

## Related Configurations

Other configuration fields (such as `InfrastructureConfig` and `Authorization`) provide ways to configure separate restaurant environments easily.

- For details on database settings, see `0002-database-configuration`.
- For details on identity and Keycloak settings, see `0003-keycloak-configuration`.
