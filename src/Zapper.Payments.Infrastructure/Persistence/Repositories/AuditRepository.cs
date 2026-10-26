using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Application.Interfaces;
using Zapper.Payments.Domain.Entities;

namespace Zapper.Payments.Infrastructure.Persistence.Repositories
{
    public sealed class AuditRepository(
        PaymentsDbContext db)
        : IAuditRepository
    {
        public void Add(AcmeAttempt attempt) =>
            db.AcmeAttempts.Add(attempt);
    }
}
