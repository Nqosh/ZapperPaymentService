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
    public sealed class NotificationRepository(
        PaymentsDbContext db)
        : INotificationRepository
    {
        public Task<bool> ExistsAsync(
            string id,
            CancellationToken ct) =>
            db.AcmeNotifications.AnyAsync(
                x => x.NotificationId == id,
                ct);

        public void Add(AcmeNotification notification) =>
            db.AcmeNotifications.Add(notification);
    }
}
