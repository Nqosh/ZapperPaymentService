using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Application.Contracts;
using Zapper.Payments.Domain.Entities;

namespace Zapper.Payments.Application.Interfaces
{
    public interface IAcmeClient
    {
        Task<AcmeSubmissionResult> SubmitAsync(
            Payment payment,
            StoredCard card,
            SubMerchant merchant,
            CancellationToken ct);
    }
}
