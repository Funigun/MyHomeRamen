using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Persistance.Payments.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RestaurantId)
               .IsRequired();

        builder.Property(x => x.FirstName)
               .IsRequired()
               .HasMaxLength(UserConstants.MaxFirstNameLength);

        builder.Property(x => x.LastName)
               .IsRequired()
               .HasMaxLength(UserConstants.MaxLastNameLength);

        builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(UserConstants.MaxEmailLength);

        builder.Property(x => x.PhoneNumber)
               .IsRequired()
               .HasMaxLength(UserConstants.MaxPhoneNumberLength);

        builder.HasOne(x => x.DefaultMethod)
               .WithMany()
               .IsRequired(false);

        builder.HasMany(x => x.Roles)
               .WithMany()
               .UsingEntity(j => j.ToTable("UserRoles"));

        builder.HasMany(x => x.Permissions)
               .WithMany()
               .UsingEntity(j => j.ToTable("UserPermissions"));
    }
}
