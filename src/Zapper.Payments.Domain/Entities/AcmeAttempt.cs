using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Domain.Entities
{
    public sealed class AcmeAttempt
    {
        public long Id { get; private set; }

        public Guid PaymentId { get; private set; }

        public Payment Payment { get; private set; } = null!;

        public DateTimeOffset StartedAt { get; private set; }

        public DateTimeOffset CompletedAt { get; private set; }

        public int? StatusCode { get; private set; }

        public string Outcome { get; private set; } = null!;

        public string? Error { get; private set; }

        private AcmeAttempt()
        {
        }

        public AcmeAttempt(
            Guid paymentId,
            DateTimeOffset started,
            DateTimeOffset completed,
            int? status,
            string outcome,
            string? error)
        {
            PaymentId = paymentId;
            StartedAt = started;
            CompletedAt = completed;
            StatusCode = status;
            Outcome = outcome;
            Error = error;
        }
    }
}
