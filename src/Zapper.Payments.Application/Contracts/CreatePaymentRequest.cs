using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Application.Contracts
{
    public sealed record CreatePaymentRequest(
        string CustomerId,
        string SubMerchantId,
        decimal Amount,
        string Currency,
        string AppPaymentId);
}
