using MyHomeRamen.Blazor.Features.Account.Common.Models;

namespace MyHomeRamen.Blazor.Features.Account.AccountManagement.Components;

public sealed class AddressFormModel
{
    public Guid? Id { get; set; }

    public string Street { get; set; } = string.Empty;

    public string Building { get; set; } = string.Empty;

    public string? Apartment { get; set; }

    public string City { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public AddAddressRequest ToAddRequest()
    {
        return new AddAddressRequest(Street, Building, Apartment, City, ZipCode, IsDefault);
    }

    public UpdateAddressRequest ToUpdateRequest()
    {
        if (Id is null)
        {
            throw new InvalidOperationException("Id must be set to create an update request.");
        }

        return new UpdateAddressRequest(Street, Building, Apartment, City, ZipCode, IsDefault);
    }

    public static AddressFormModel FromDto(AddressDto dto)
    {
        return new AddressFormModel
        {
            Id = dto.Id,
            Street = dto.Street,
            Building = dto.Building,
            Apartment = dto.Apartment,
            City = dto.City,
            ZipCode = dto.ZipCode,
            IsDefault = dto.IsDefault
        };
    }

    public static AddressFormModel Empty()
    {
        return new AddressFormModel();
    }
}
