using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Application.Contracts;
using Zapper.Payments.Application.Interfaces;
using Zapper.Payments.Domain.Entities;

namespace Zapper.Payments.Application.Services
{
    public sealed class PaymentService(
    ICustomerRepository customers,
    ISubMerchantRepository merchants,
    IPaymentRepository payments,
    IAuditRepository audits,
    IUnitOfWork uow,
    IAcmeClient acme)
    : IPaymentService
    {
        public async Task<ServiceResult<PaymentDto>> CreateAsync(
            CreatePaymentRequest r,
            CancellationToken ct)
        {
            if (r.Amount <= 0)
            {
                return new(null, "Amount must be greater than zero");
            }

            if (string.IsNullOrWhiteSpace(r.AppPaymentId))
            {
                return new(null, "App payment identifier is required");
            }

            var existing = await payments.GetByAppIdAsync(
                r.AppPaymentId,
                false,
                ct);

            if (existing is not null)
            {
                return new(Map(existing), null, false);
            }

            var customer = await customers.GetWithCardsAsync(
                r.CustomerId,
                ct);

            if (customer is null)
            {
                return new(null, "Customer not found");
            }

            var card = customer.StoredCards
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (card is null)
            {
                return new(null, "Customer has no stored card");
            }

            var merchant = await merchants.GetAsync(
                r.SubMerchantId,
                ct);

            if (merchant is null)
            {
                return new(null, "Sub-merchant not found");
            }

            var now = DateTimeOffset.UtcNow;

            var payment = new Payment(
                r.AppPaymentId,
                customer,
                merchant,
                r.Amount,
                r.Currency,
                now);

            payments.Add(payment);

            try
            {
                await uow.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                var raced = await payments.GetByAppIdAsync(
                    r.AppPaymentId,
                    false,
                    ct);

                if (raced is not null)
                {
                    return new(Map(raced), null, false);
                }

                throw;
            }

            var started = DateTimeOffset.UtcNow;

            var result = await acme.SubmitAsync(
                payment,
                card,
                merchant,
                ct);

            var completed = DateTimeOffset.UtcNow;

            audits.Add(
                new AcmeAttempt(
                    payment.Id,
                    started,
                    completed,
                    result.StatusCode,
                    result.TimedOut
                        ? "TimedOut"
                        : result.Success
                            ? "Accepted"
                            : "Failed",
                    result.Error));

            if (result.Success)
            {
                payment.SetAcmeId(result.PaymentId!);
                payment.MarkSubmitted(completed);
            }
            else if (result.TimedOut)
            {
                payment.MarkSubmissionTimedOut(completed);
            }
            else
            {
                payment.MarkSubmissionFailed(
                    completed,
                    result.Error);
            }

            await uow.SaveChangesAsync(ct);

            return new(Map(payment), null, true);
        }

        public async Task<PaymentDetailsDto?> GetAsync(
            string id,
            CancellationToken ct)
        {
            var payment = await payments.GetByAppIdAsync(
                id,
                false,
                ct);

            return payment is null
                ? null
                : new PaymentDetailsDto(
                    payment.AppPaymentId,
                    payment.AcmePaymentId,
                    payment.Amount,
                    payment.Currency,
                    payment.Status.ToString(),
                    payment.CreatedAt,
                    payment.UpdatedAt,
                    payment.Events
                        .OrderBy(x => x.OccurredAt)
                        .Select(x => new PaymentEventDto(
                            x.Type.ToString(),
                            x.OccurredAt,
                            x.Detail))
                        .ToArray());
        }

        private static PaymentDto Map(Payment payment) =>
            new(
                payment.AppPaymentId,
                payment.AcmePaymentId,
                payment.Amount,
                payment.Currency,
                payment.Status.ToString(),
                payment.CreatedAt,
                payment.UpdatedAt);
    }
}
