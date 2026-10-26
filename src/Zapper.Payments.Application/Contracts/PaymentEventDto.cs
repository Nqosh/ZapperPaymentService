using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Application.Contracts
{
    public sealed record PaymentEventDto(
       string Type,
       DateTimeOffset OccurredAt,
       string? Detail);
}
