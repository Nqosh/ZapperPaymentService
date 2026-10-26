using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Application.Contracts
{
    public sealed record PaymentDetailsDto(
       string AppPaymentId,
       string? AcmePaymentId,
       decimal Amount,
       string Currency,
       string Status,
       DateTimeOffset CreatedAt,
       DateTimeOffset UpdatedAt,
       IReadOnlyCollection<PaymentEventDto> Events);
}
