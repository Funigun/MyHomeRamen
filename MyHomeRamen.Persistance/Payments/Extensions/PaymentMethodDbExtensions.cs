using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.PaymentMethods;

namespace MyHomeRamen.Persistance.Payments.Extensions;

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
    }
}
