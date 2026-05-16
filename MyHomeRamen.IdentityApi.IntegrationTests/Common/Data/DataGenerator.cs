using MyHomeRamen.Api.Users.Features.Account.CreateAddress.Models;
using MyHomeRamen.Api.Users.Features.Account.UpdateAddress.Models;
using MyHomeRamen.Common.Contracts.Account;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

internal static class DataGenerator
{
    internal static AddAddressRequest GenerateValidAddAddressRequest(bool isDefault = false)
    {
        return new AddAddressRequest(
            Street: "Main Street",
            Building: "10A",
            Apartment: "5",
            City: "Warsaw",
            ZipCode: "00-001",
            IsDefault: isDefault);
    }

    public static TheoryData<AddAddressRequest> InvalidAddAddressRequests() => new()
    {
        new AddAddressRequest(string.Empty, "10A", "5", "Warsaw", "00-001", false),
        new AddAddressRequest(new string('a', AddressValidationExtensions.MaxStreetLength + 1), "10A", "5", "Warsaw", "00-001", false),
        new AddAddressRequest("Main Street", string.Empty, "5", "Warsaw", "00-001", false),
        new AddAddressRequest("Main Street", new string('a', AddressValidationExtensions.MaxBuildingLength + 1), "5", "Warsaw", "00-001", false),
        new AddAddressRequest("Main Street", "10A", "5", string.Empty, "00-001", false),
        new AddAddressRequest("Main Street", "10A", "5", "Warsaw", string.Empty, false),
    };

    internal static UpdateAddressRequest GenerateValidUpdateAddressRequest(bool isDefault = false)
    {
        return new UpdateAddressRequest(
            Id: Guid.Empty,
            Street: "Updated Street",
            Building: "5B",
            Apartment: "3",
            City: "Gdansk",
            ZipCode: "80-001",
            IsDefault: isDefault);
    }

    public static TheoryData<UpdateAddressRequest> InvalidUpdateAddressRequests() => new()
    {
        new UpdateAddressRequest(Guid.Empty, string.Empty, "10A", "5", "Warsaw", "00-001", false),
        new UpdateAddressRequest(Guid.Empty, new string('a', AddressValidationExtensions.MaxStreetLength + 1), "10A", "5", "Warsaw", "00-001", false),
        new UpdateAddressRequest(Guid.Empty, "Main Street", string.Empty, "5", "Warsaw", "00-001", false),
        new UpdateAddressRequest(Guid.Empty, "Main Street", new string('a', AddressValidationExtensions.MaxBuildingLength + 1), "5", "Warsaw", "00-001", false),
        new UpdateAddressRequest(Guid.Empty, "Main Street", "10A", "5", string.Empty, "00-001", false),
        new UpdateAddressRequest(Guid.Empty, "Main Street", "10A", "5", "Warsaw", string.Empty, false),
    };
}
