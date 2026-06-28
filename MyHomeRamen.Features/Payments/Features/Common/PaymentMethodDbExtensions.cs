using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.PaymentMethods;

namespace MyHomeRamen.Features.Payments.Features.Common;

public static class PaymentMethodDbExtensions
{
    extension(IQueryable<PaymentMethod> query)
    {
        public IQueryable<PaymentMethod> GetAvailableMethods()
        {
            return query.AsNoTracking()
                        .Include(method => method.PaymentChannels.OrderBy(channel => channel.DisplayOrder))
                        .Where(method => method.IsActive)
                        .OrderBy(method => method.DisplayOrder);
        }

        public IQueryable<PaymentMethod> GetById(PaymentMethodId id)
        {
            return query.AsNoTracking()
                        .Include(method => method.PaymentChannels)
                        .Where(method => method.Id == id && method.IsActive);
        }
    }
}
