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
    public sealed class CustomerRepository(
        PaymentsDbContext db)
        : ICustomerRepository
    {
        public Task<Customer?> GetWithCardsAsync(
            string id,
            CancellationToken ct) =>
            db.Customers
                .Include(x => x.StoredCards)
                .SingleOrDefaultAsync(
                    x => x.ExternalId == id,
                    ct);
    }

}
