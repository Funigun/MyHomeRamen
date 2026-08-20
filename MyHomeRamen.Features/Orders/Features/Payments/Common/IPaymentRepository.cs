using MyHomeRamen.Domain.Orders.Payments;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Orders.Features.Payments.Common;

public interface IPaymentRepository : IRepository<Payment, PaymentId>
{
    IPaymentQuery Query();

    IPaymentLoader Load();
}
