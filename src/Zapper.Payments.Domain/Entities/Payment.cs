using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Domain.Enums;

namespace Zapper.Payments.Domain.Entities
{
    public sealed class Payment
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string AppPaymentId { get; private set; } = null!;

        public string? AcmePaymentId { get; private set; }

        public Guid CustomerId { get; private set; }

        public Customer Customer { get; private set; } = null!;

        public Guid SubMerchantId { get; private set; }

        public SubMerchant SubMerchant { get; private set; } = null!;

        public decimal Amount { get; private set; }

        public string Currency { get; private set; } = null!;

        public PaymentStatus Status { get; private set; }
            = PaymentStatus.Pending;

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset UpdatedAt { get; private set; }

        public List<PaymentEvent> Events { get; private set; } = [];

        private Payment()
        {
        }

        public Payment(
            string appId,
            Customer customer,
            SubMerchant merchant,
            decimal amount,
            string currency,
            DateTimeOffset now)
        {
            AppPaymentId = appId;
            Customer = customer;
            CustomerId = customer.Id;
            SubMerchant = merchant;
            SubMerchantId = merchant.Id;
            Amount = amount;
            Currency = currency.ToUpperInvariant();
            CreatedAt = now;
            UpdatedAt = now;

            AddEvent(
                PaymentEventType.Created,
                now,
                "Payment accepted locally");
        }

        public void SetAcmeId(string id) =>
            AcmePaymentId ??= id;

        public void MarkSubmitted(DateTimeOffset at) =>
            AddEvent(
                PaymentEventType.Submitted,
                at,
                "Accepted by Acme");

        public void MarkSubmissionTimedOut(DateTimeOffset at) =>
            AddEvent(
                PaymentEventType.SubmissionTimedOut,
                at,
                "Outcome unknown; retained as Pending");

        public void MarkSubmissionFailed(
            DateTimeOffset at,
            string? reason) =>
            AddEvent(
                PaymentEventType.SubmissionFailed,
                at,
                reason);

        public bool Apply(
            PaymentStatus incoming,
            DateTimeOffset occurredAt,
            string notificationId)
        {
            var validTransition =
                PaymentTransitionPolicy.CanTransition(
                    Status,
                    incoming);

            AddEvent(
                validTransition
                    ? Enum.Parse<PaymentEventType>(incoming.ToString())
                    : PaymentEventType.IgnoredTransition,
                occurredAt,
                validTransition
                    ? null
                    : $"Ignored {Status} -> {incoming}",
                notificationId);

            if (validTransition)
            {
                Status = incoming;
                UpdatedAt = DateTimeOffset.UtcNow;
            }

            return validTransition;
        }

        private void AddEvent(
            PaymentEventType type,
            DateTimeOffset at,
            string? detail = null,
            string? notificationId = null)
        {
            Events.Add(
                new PaymentEvent(
                    Id,
                    type,
                    Amount,
                    at,
                    detail,
                    notificationId));
        }
    }
}
