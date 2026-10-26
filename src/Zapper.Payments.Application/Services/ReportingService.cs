using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Application.Contracts;
using Zapper.Payments.Application.Interfaces;

namespace Zapper.Payments.Application.Services
{
    public sealed class ReportingService(
        IReportRepository reports)
        : IReportingService
    {
        public Task<IReadOnlyCollection<DailySummaryRow>> GetDailyAsync(
            DateOnly date,
            CancellationToken ct) =>
            reports.GetDailyAsync(date, ct);
    }
}
