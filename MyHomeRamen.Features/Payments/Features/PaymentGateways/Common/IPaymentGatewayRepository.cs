using MyHomeRamen.Domain.Payments.PaymentGateways;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;

public interface IPaymentGatewayRepository : IRepository<PaymentGateway, PaymentGatewayId>, IPaymentGatewayQuery, IPaymentGatewaySpecification
{
    IPaymentGatewayQuery Query();

    IPaymentGatewaySpecification Specification();
}
