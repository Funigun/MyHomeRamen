namespace MyHomeRamen.Domain.Abstractions;

public abstract class AuditableEntity
{
    public string CreatedBy { get; set; } = default!;

    public DateTimeOffset CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTimeOffset? ModifiedOn { get; set; }
}
