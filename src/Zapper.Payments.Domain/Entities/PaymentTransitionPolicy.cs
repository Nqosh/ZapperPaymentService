using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Domain.Enums;

namespace Zapper.Payments.Domain.Entities
{
    public static class PaymentTransitionPolicy
    {
        public static bool CanTransition(
            PaymentStatus current,
            PaymentStatus incoming) =>
            (current, incoming) switch
            {
                (PaymentStatus.Pending, PaymentStatus.Approved) => true,
                (PaymentStatus.Pending, PaymentStatus.Declined) => true,
                (PaymentStatus.Pending, PaymentStatus.Reversed) => true,
                (PaymentStatus.Pending, PaymentStatus.Refunded) => true,
                (PaymentStatus.Approved, PaymentStatus.Reversed) => true,
                (PaymentStatus.Approved, PaymentStatus.Refunded) => true,
                _ => false
            };
    }
}
