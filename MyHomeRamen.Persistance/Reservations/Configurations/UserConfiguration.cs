using MyHomeRamen.Domain.Common.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Reservations.Users;

namespace MyHomeRamen.Persistance.Reservations.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(UserConstants.MaxFirstNameLength);

        builder.Property(x => x.LastName)
            .HasMaxLength(UserConstants.MaxLastNameLength);

        builder.Property(x => x.Email)
            .HasMaxLength(UserConstants.MaxEmailLength);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(UserConstants.MaxPhoneNumberLength);

        builder.HasMany(x => x.Bookings)
            .WithOne();

        builder.HasMany(x => x.Roles)
            .WithMany();

        builder.HasMany(x => x.Permissions)
            .WithMany();
    }
}
