using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

namespace MyHomeRamen.Domain.Restaurants.Restaurants;

public sealed class Restaurant : Aggregate<RestaurantId>
{
    private readonly List<WorkingHours> _workHours = [];
    private readonly List<ClosingPeriod> _closingPeriods = [];

    public string Name { get; private set; } = default!;

    public bool IsActive { get; private set; }

    public Address Address { get; private set; } = default!;

    public ContactDetails ContactDetails { get; private set; } = default!;

    public BankAccount BankAccount { get; private set; } = default!;

    public IReadOnlyList<WorkingHours> WorkHours => _workHours.ToList();

    public IReadOnlyList<ClosingPeriod> ClosingPeriods => _closingPeriods.ToList();

    private Restaurant () { }

    public static Restaurant Create(string name, Address address, bool isActive)
    {
        Restaurant restaurant = new()
        {
            Id = new RestaurantId(Guid.CreateVersion7()),
            Name = name,
            Address = address,
            IsActive = isActive,
        };

        RestaurantValidator.Validate(restaurant);
        return restaurant;
    }

    public void UpdateDetails(string name, Address address, bool isActive)
    {
        Name = name;
        Address = address;
        IsActive = isActive;
        RestaurantValidator.Validate(this);
    }

    public void UpdateContactDetails(string phone, string email)
    {
        ContactDetails = ContactDetails.Create(phone, email);
    }

    public void UpdateBankAccount(string accountNumber, string bankName, string routingNumber)
    {
        BankAccount = BankAccount.Create(accountNumber, bankName, routingNumber);
    }

    public void AddWorkingHours(IEnumerable<WorkingHours> workingHours)
    {
        _workHours.Clear();
        _workHours.AddRange(workingHours);
    }

    public void AddClosingPeriod(ClosingPeriod closingPeriod) => _closingPeriods.Add(closingPeriod);
    
    public void UpdateClosingPeriod(ClosingPeriodId closingPeriodId, DateTimeOffset startTime, DateTimeOffset endTime, string reason)
    {
        ClosingPeriod? closingPeriod = _closingPeriods.FirstOrDefault(cp => cp.Id == closingPeriodId);
        closingPeriod?.UpdatePeriod(startTime, endTime, reason);        
    }

    public void DeactivateClosingPeriod(ClosingPeriodId closingPeriodId)
    {
        ClosingPeriod? closingPeriod = _closingPeriods.FirstOrDefault(cp => cp.Id == closingPeriodId);
        closingPeriod?.Deactivate();
    }
}
