using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace MyHomeRamen.Identity.Api.Persistance;

public class GuidFinalizingConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            foreach (IConventionProperty property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(Guid) &&
                    property.ValueGenerated == ValueGenerated.OnAdd)
                {
                    property.SetValueGeneratorFactory((_, _) => new GuidValueGenerator());
                }
            }
        }
    }
}
