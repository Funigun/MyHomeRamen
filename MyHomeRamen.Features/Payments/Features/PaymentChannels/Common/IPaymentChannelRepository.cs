using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;

public interface IPaymentChannelRepository : IRepository<PaymentChannel, PaymentChannelId>, IPaymentChannelQuery, IPaymentChannelSpecification
{
    IPaymentChannelQuery Query();

    IPaymentChannelSpecification Specification();
}
