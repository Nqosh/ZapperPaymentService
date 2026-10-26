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
    public sealed class PaymentRepository(
        PaymentsDbContext db)
        : IPaymentRepository
    {
        public Task<Payment?> GetByAppIdAsync(
            string id,
            bool tracking,
            CancellationToken ct)
        {
            var query = db.Payments
                .Include(x => x.Events)
                .AsQueryable();

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            return query.SingleOrDefaultAsync(
                x => x.AppPaymentId == id,
                ct);
        }

        public Task<Payment?> GetByAcmeOrAppIdAsync(
            string acmeId,
            string appId,
            CancellationToken ct) =>
            db.Payments
                .Include(x => x.Events)
                .SingleOrDefaultAsync(
                    x => x.AcmePaymentId == acmeId ||
                         x.AppPaymentId == appId,
                    ct);

        public void Add(Payment payment) =>
            db.Payments.Add(payment);
    }
}
