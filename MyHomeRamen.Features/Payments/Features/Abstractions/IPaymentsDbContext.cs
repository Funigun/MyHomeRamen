using MyHomeRamen.Features.Payments.Features.Orders.Common;
using MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;
using MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;
using MyHomeRamen.Features.Payments.Features.Permissions.Common;
using MyHomeRamen.Features.Payments.Features.Roles.Common;
using MyHomeRamen.Features.Payments.Features.Users.Common;

namespace MyHomeRamen.Features.Payments.Features.Abstractions;

public interface IPaymentsDbContext : IPaymentsUnitOfWork
{
    IPaymentMethodRepository PaymentMethod { get; }

    IPaymentChannelRepository PaymentChannel { get; }

    IPaymentGatewayRepository PaymentGateway { get; }

    IOrderRepository Order { get; }

    IUserRepository User { get; }

    IRoleRepository Role { get; }

    IPermissionRepository Permission { get; }
}
