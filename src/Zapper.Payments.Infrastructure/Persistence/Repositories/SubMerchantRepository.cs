using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Application.Interfaces;
using Zapper.Payments.Domain.Entities;

namespace Zapper.Payments.Infrastructure.Persistence.Repositories
{
    public sealed class SubMerchantRepository(
       PaymentsDbContext db)
       : ISubMerchantRepository
    {
        public Task<SubMerchant?> GetAsync(
            string id,
            CancellationToken ct) =>
            db.SubMerchants
                .SingleOrDefaultAsync(
                    x => x.ExternalId == id,
                    ct);
    }

}
