using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Identity.Roles;

public class Role : AuditableEntity, IEntity<RoleId>
{
    public RoleId Id { get; private set; }

    public string Name { get; private set; } = default!;

    public string Description { get; private set; } = default!;

    private Role()
    {
        
    }

    public static Role CreateForSeed(string name, string description)
    {
        Role role = new Role
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Description = description
        };

        return role;
    }

    public static Role CreateForTest(string name)
    {
        Role role = new Role
        {
            Name = name,
        };

        return role;
    }
}
