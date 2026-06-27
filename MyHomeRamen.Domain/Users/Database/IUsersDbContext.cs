using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Users.Database;

public interface IUsersDbContext : IBaseDbContext
{
    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<Address> Addresses { get; }
}
