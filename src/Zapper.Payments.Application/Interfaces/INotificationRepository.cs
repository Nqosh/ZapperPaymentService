using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Domain.Entities;

namespace Zapper.Payments.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task<bool> ExistsAsync(
            string notificationId,
            CancellationToken ct);

        void Add(AcmeNotification notification);
    }
}
