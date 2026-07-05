using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Features.Payments.Features.Orders.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IOrderSpecification
{
    async Task<Order?> IOrderSpecification.ByIdAsync(OrderId id, CancellationToken cancellationToken)
        => await Orders.AsNoTracking().FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
}
