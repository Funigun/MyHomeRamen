using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Domain.Common.Address;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

internal static class DataGenerator
{
    internal static CreateAddressRequest GenerateValidAddAddressRequest(bool isDefault = false)
    {
        return new CreateAddressRequest(
            Street: "Main Street",
            Building: "10A",
            Apartment: "5",
            City: "Warsaw",
            ZipCode: "00-001",
            IsDefault: isDefault);
    }

    public static TheoryData<CreateAddressRequest> InvalidAddAddressRequests() => new()
    {
        new CreateAddressRequest(string.Empty, "10A", "5", "Warsaw", "00-001", false),
        new CreateAddressRequest(new string('a', AddressConstants.MaxStreetLength + 1), "10A", "5", "Warsaw", "00-001", false),
        new CreateAddressRequest("Main Street", string.Empty, "5", "Warsaw", "00-001", false),
        new CreateAddressRequest("Main Street", new string('a', AddressConstants.MaxBuildingLength + 1), "5", "Warsaw", "00-001", false),
        new CreateAddressRequest("Main Street", "10A", "5", string.Empty, "00-001", false),
        new CreateAddressRequest("Main Street", "10A", "5", "Warsaw", string.Empty, false),
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
        new UpdateAddressRequest(Guid.Empty, new string('a', AddressConstants.MaxStreetLength + 1), "10A", "5", "Warsaw", "00-001", false),
        new UpdateAddressRequest(Guid.Empty, "Main Street", string.Empty, "5", "Warsaw", "00-001", false),
        new UpdateAddressRequest(Guid.Empty, "Main Street", new string('a', AddressConstants.MaxBuildingLength + 1), "5", "Warsaw", "00-001", false),
        new UpdateAddressRequest(Guid.Empty, "Main Street", "10A", "5", string.Empty, "00-001", false),
        new UpdateAddressRequest(Guid.Empty, "Main Street", "10A", "5", "Warsaw", string.Empty, false),
    };
}
