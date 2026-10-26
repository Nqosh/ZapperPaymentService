using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Zapper.Payments.Application.Contracts;
using Zapper.Payments.Application.Interfaces;
using Zapper.Payments.Domain.Entities;
using Zapper.Payments.Domain.Enums;

namespace Zapper.Payments.Application.Services
{
    public sealed class NotificationService(
      INotificationRepository notifications,
      IPaymentRepository payments,
      IUnitOfWork uow,
      ICallbackSignatureVerifier verifier)
      : INotificationService
    {
        public bool IsValidSignature(
            string raw,
            string? signature) =>
            verifier.IsValid(raw, signature);

        public async Task<string> ProcessAsync(
            string raw,
            CancellationToken ct)
        {
            var notification =
                JsonSerializer.Deserialize<AcmeNotificationRequest>(
                    raw,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web))
                ?? throw new InvalidOperationException(
                    "Invalid notification body");

            if (await notifications.ExistsAsync(
                    notification.NotificationId,
                    ct))
            {
                return "duplicate";
            }

            var payment = await payments.GetByAcmeOrAppIdAsync(
                notification.AcmePaymentId,
                notification.AppPaymentId,
                ct);

            var inbox = new AcmeNotification(
                notification.NotificationId,
                notification.AcmePaymentId,
                notification.EventType,
                notification.OccurredAt,
                raw);

            notifications.Add(inbox);

            if (payment is null)
            {
                inbox.Mark(false, "Unknown payment");

                await uow.SaveChangesAsync(ct);

                return "unknown-payment";
            }

            if (!Enum.TryParse<PaymentStatus>(
                    notification.EventType,
                    true,
                    out var incoming) ||
                incoming == PaymentStatus.Pending)
            {
                inbox.Mark(false, "Unsupported event");

                await uow.SaveChangesAsync(ct);

                return "unsupported";
            }

            payment.SetAcmeId(notification.AcmePaymentId);

            var applied = payment.Apply(
                incoming,
                notification.OccurredAt,
                notification.NotificationId);

            inbox.Mark(
                applied,
                applied
                    ? null
                    : $"Ignored transition to {incoming}");

            await uow.SaveChangesAsync(ct);

            return applied
                ? "applied"
                : "ignored";
        }
    }
}
