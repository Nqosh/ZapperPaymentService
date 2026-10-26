using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Application.Interfaces
{
    public interface INotificationService
    {
        bool IsValidSignature(
            string raw,
            string? signature);

        Task<string> ProcessAsync(
            string raw,
            CancellationToken ct);
    }
}
