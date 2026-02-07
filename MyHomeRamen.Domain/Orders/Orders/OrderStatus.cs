namespace MyHomeRamen.Domain.Orders.Orders;

public enum OrderStatus
{
    Created = 0,
    Confirmed = 1,
    Preparing = 2,
    DeliveryInProgress = 3,
    ReadyForPickup = 4,
    Completed = 5,
    Rejected = 6,
    Cancelled = 7
}
