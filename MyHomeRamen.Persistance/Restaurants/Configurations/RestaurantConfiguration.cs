using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.Restaurant;
using MyHomeRamen.Domain.Restaurants.Restaurants;

namespace MyHomeRamen.Persistance.Restaurants.Configurations;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(RestaurantConstants.MaxNameLength);

        builder.Property(x => x.IsActive)
               .IsRequired();

                builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(x => x.Street)
                   .HasColumnName(nameof(Restaurant.Address.Street))
                   .IsRequired()
                   .HasMaxLength(RestaurantConstants.MaxStreetLength);

            address.Property(x => x.City)
                   .HasColumnName(nameof(Restaurant.Address.City))
                   .IsRequired()
                   .HasMaxLength(RestaurantConstants.MaxCityLength);

            address.Property(x => x.ZipCode)
                   .HasColumnName(nameof(Restaurant.Address.ZipCode))
                   .IsRequired()
                   .HasMaxLength(RestaurantConstants.MaxZipCodeLength);

            address.OwnsOne(x => x.Location, location =>
            {
                location.Property(x => x.Latitude)
                        .HasColumnName(nameof(Restaurant.Address.Location.Latitude))
                        .IsRequired();

                location.Property(x => x.Longitude)
                        .HasColumnName(nameof(Restaurant.Address.Location.Longitude))
                        .IsRequired();
            });
        });

        builder.OwnsOne(x => x.ContactDetails, contactDetails =>
        {
            contactDetails.Property(x => x.Phone)
                          .HasColumnName(nameof(Restaurant.ContactDetails.Phone))
                          .IsRequired()
                          .HasMaxLength(RestaurantConstants.MaxPhoneLength);

            contactDetails.Property(x => x.Email)
                          .HasColumnName(nameof(Restaurant.ContactDetails.Email))
                          .IsRequired()
                          .HasMaxLength(RestaurantConstants.MaxEmailLength);
        });

        builder.OwnsOne(x => x.BankAccount, bankAccount =>
        {
            bankAccount.Property(x => x.AccountNumber)
                       .HasColumnName(nameof(Restaurant.BankAccount.AccountNumber))
                       .IsRequired()
                       .HasMaxLength(RestaurantConstants.MaxAccountNumberLength);

            bankAccount.Property(x => x.BankName)
                       .HasColumnName(nameof(Restaurant.BankAccount.BankName))
                       .IsRequired()
                       .HasMaxLength(RestaurantConstants.MaxBankNameLength);

            bankAccount.Property(x => x.RoutingNumber)
                       .HasColumnName(nameof(Restaurant.BankAccount.RoutingNumber))
                       .IsRequired()
                       .HasMaxLength(RestaurantConstants.MaxRoutingNumberLength);
        });

        builder.OwnsMany(x => x.WorkHours, workHours =>
        {
            workHours.ToTable("RestaurantWorkingHours");

            workHours.Property(x => x.Day)
                     .IsRequired();

            workHours.Property(x => x.OpenTime)
                     .IsRequired();

            workHours.Property(x => x.CloseTime)
                     .IsRequired();

            workHours.WithOwner()
                     .HasForeignKey("RestaurantId");
        });

        builder.Navigation(x => x.WorkHours).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.ClosingPeriods)
               .WithOne()
               .HasForeignKey("RestaurantId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.ClosingPeriods).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
