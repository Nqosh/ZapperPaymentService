using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Domain.Enums
{
    public enum PaymentEventType
    {
        Created,
        Submitted,
        SubmissionTimedOut,
        SubmissionFailed,
        Approved,
        Declined,
        Reversed,
        Refunded,
        IgnoredTransition
    }
}
