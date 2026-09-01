using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Payments.Features.Orders.Common;
using MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;
using MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;

namespace MyHomeRamen.Features.Payments.Features.Abstractions;

public interface IPaymentsDbContext : IUnitOfWork
{
    IPaymentMethodRepository PaymentMethod { get; }

    IPaymentChannelRepository PaymentChannel { get; }

    IPaymentGatewayRepository PaymentGateway { get; }

    IOrderRepository Order { get; }
}
