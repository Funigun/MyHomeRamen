using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;

public interface IPaymentMethodRepository : IRepository<PaymentMethod, PaymentMethodId>, IPaymentMethodQuery, IPaymentMethodSpecification
{
    IPaymentMethodQuery Query();

    IPaymentMethodSpecification Specification();
}
