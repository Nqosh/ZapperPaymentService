using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Application.Contracts;

namespace Zapper.Payments.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ServiceResult<PaymentDto>> CreateAsync(
            CreatePaymentRequest request,
            CancellationToken ct);

        Task<PaymentDetailsDto?> GetAsync(
            string appPaymentId,
            CancellationToken ct);
    }
}
