using Microsoft.EntityFrameworkCore;

using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.GetAvailableMethods;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IPaymentMethodQuery
{
    private IQueryable<PaymentMethod> PaymentMethodsQuery => PaymentMethods.AsNoTracking();

    public async Task<List<GetAvailableMethodsResponse>> GetAvailableMethodsAsync(CancellationToken cancellationToken)
        => await PaymentMethodsQuery
            .Include(method => method.PaymentChannels.OrderBy(channel => channel.DisplayOrder))
            .Where(method => method.IsActive)
            .OrderBy(method => method.DisplayOrder)
            .Select(method => new GetAvailableMethodsResponse(
                method.Id,
                method.Name,
                method.ImageUrl,
                method.PaymentChannels.Select(channel => new AvailableChannelDto(channel.Id, channel.Name, channel.ImageUrl))))
            .ToListAsync(cancellationToken);

    public async Task<PaymentMethod?> GetByIdAsync(PaymentMethodId id, CancellationToken cancellationToken)
        => await PaymentMethodsQuery
            .Include(method => method.PaymentChannels)
            .Where(method => method.Id == id && method.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
}
