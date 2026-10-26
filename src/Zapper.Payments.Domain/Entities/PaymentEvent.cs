using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Domain.Enums;

namespace Zapper.Payments.Domain.Entities
{
    public sealed class PaymentEvent
    {
        public long Id { get; private set; }

        public Guid PaymentId { get; private set; }

        public Payment Payment { get; private set; } = null!;

        public PaymentEventType Type { get; private set; }

        public decimal Amount { get; private set; }

        public DateTimeOffset OccurredAt { get; private set; }

        public string? Detail { get; private set; }

        public string? AcmeNotificationId { get; private set; }

        private PaymentEvent()
        {
        }

        internal PaymentEvent(
            Guid paymentId,
            PaymentEventType type,
            decimal amount,
            DateTimeOffset at,
            string? detail,
            string? notificationId)
        {
            PaymentId = paymentId;
            Type = type;
            Amount = amount;
            OccurredAt = at;
            Detail = detail;
            AcmeNotificationId = notificationId;
        }
    }
}
