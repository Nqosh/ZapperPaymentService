using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Domain.Entities;

namespace Zapper.Payments.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByAppIdAsync(
            string appId,
            bool tracking,
            CancellationToken ct);

        Task<Payment?> GetByAcmeOrAppIdAsync(
            string acmeId,
            string appId,
            CancellationToken ct);

        void Add(Payment payment);
    }
}
