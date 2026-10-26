using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Application.Contracts
{
    public sealed record AcmeNotificationRequest(
       string NotificationId,
       string AcmePaymentId,
       string AppPaymentId,
       string EventType,
       DateTimeOffset OccurredAt);
}
