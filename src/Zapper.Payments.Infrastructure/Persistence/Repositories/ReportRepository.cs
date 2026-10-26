using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Application.Contracts;
using Zapper.Payments.Application.Interfaces;
using Zapper.Payments.Domain.Enums;

namespace Zapper.Payments.Infrastructure.Persistence.Repositories
{

    public sealed class ReportRepository(
        PaymentsDbContext db)
        : IReportRepository
    {
        public async Task<IReadOnlyCollection<DailySummaryRow>> GetDailyAsync(
        DateOnly date,
        CancellationToken ct)
        {
            var from = new DateTimeOffset(
                date.ToDateTime(TimeOnly.MinValue),
                TimeSpan.Zero);

            var to = from.AddDays(1);

            var grouped = await db.PaymentEvents
                .AsNoTracking()
                .Where(e =>
                    e.OccurredAt >= from &&
                    e.OccurredAt < to &&
                    (e.Type == PaymentEventType.Approved ||
                     e.Type == PaymentEventType.Declined ||
                     e.Type == PaymentEventType.Reversed ||
                     e.Type == PaymentEventType.Refunded))
                .GroupBy(e => new
                {
                    e.Payment.SubMerchant.ExternalId,
                    e.Payment.SubMerchant.TradingName,
                    e.Payment.Currency
                })
                .Select(g => new
                {
                    g.Key.ExternalId,
                    g.Key.TradingName,
                    g.Key.Currency,

                    ApprovedCount =
                        g.LongCount(e =>
                            e.Type == PaymentEventType.Approved),

                    ApprovedValue =
                        g.Where(e =>
                                e.Type == PaymentEventType.Approved)
                            .Select(e => (decimal?)e.Amount)
                            .Sum() ?? 0m,

                    DeclinedCount =
                        g.LongCount(e =>
                            e.Type == PaymentEventType.Declined),

                    DeclinedValue =
                        g.Where(e =>
                                e.Type == PaymentEventType.Declined)
                            .Select(e => (decimal?)e.Amount)
                            .Sum() ?? 0m,

                    ReversalCount =
                        g.LongCount(e =>
                            e.Type == PaymentEventType.Reversed),

                    ReversalValue =
                        g.Where(e =>
                                e.Type == PaymentEventType.Reversed)
                            .Select(e => (decimal?)e.Amount)
                            .Sum() ?? 0m,

                    RefundCount =
                        g.LongCount(e =>
                            e.Type == PaymentEventType.Refunded),

                    RefundValue =
                        g.Where(e =>
                                e.Type == PaymentEventType.Refunded)
                            .Select(e => (decimal?)e.Amount)
                            .Sum() ?? 0m
                })
                .ToListAsync(ct);

            return grouped
                .Select(x => new DailySummaryRow(
                    x.ExternalId,
                    x.TradingName,
                    x.Currency,
                    x.ApprovedCount,
                    x.ApprovedValue,
                    x.DeclinedCount,
                    x.DeclinedValue,
                    x.ReversalCount,
                    x.ReversalValue,
                    x.RefundCount,
                    x.RefundValue))
                .ToList();
        }
    }
}
