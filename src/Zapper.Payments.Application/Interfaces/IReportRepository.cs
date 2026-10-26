using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Application.Contracts;

namespace Zapper.Payments.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<IReadOnlyCollection<DailySummaryRow>> GetDailyAsync(
            DateOnly date,
            CancellationToken ct);
    }
}
