using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Application.Contracts
{
    public sealed record DailySummaryRow(
        string SubMerchantId,
        string TradingName,
        string Currency,
        long ApprovedCount,
        decimal ApprovedValue,
        long DeclinedCount,
        decimal DeclinedValue,
        long ReversalCount,
        decimal ReversalValue,
        long RefundCount,
        decimal RefundValue);
}
