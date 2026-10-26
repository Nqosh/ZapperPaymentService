using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Domain.Entities;

namespace Zapper.Payments.Application.Interfaces
{
    public interface ISubMerchantRepository
    {
        Task<SubMerchant?> GetAsync(
            string externalId,
            CancellationToken ct);
    }
}
