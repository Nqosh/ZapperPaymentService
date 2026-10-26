using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Domain.Entities
{
    public sealed class AcmeNotification
    {
        public long Id { get; private set; }

        public string NotificationId { get; private set; } = null!;

        public string AcmePaymentId { get; private set; } = null!;

        public string EventType { get; private set; } = null!;

        public DateTimeOffset OccurredAt { get; private set; }

        public string Payload { get; private set; } = null!;

        public bool Applied { get; private set; }

        public string? ProcessingNote { get; private set; }

        public DateTimeOffset ReceivedAt { get; private set; }
            = DateTimeOffset.UtcNow;

        private AcmeNotification()
        {
        }

        public AcmeNotification(
            string id,
            string acmeId,
            string type,
            DateTimeOffset occurred,
            string payload)
        {
            NotificationId = id;
            AcmePaymentId = acmeId;
            EventType = type;
            OccurredAt = occurred;
            Payload = payload;
        }

        public void Mark(
            bool applied,
            string? note)
        {
            Applied = applied;
            ProcessingNote = note;
        }
    }
}
