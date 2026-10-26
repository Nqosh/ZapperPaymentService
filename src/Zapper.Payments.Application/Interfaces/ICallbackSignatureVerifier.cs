using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Application.Interfaces
{
    public interface ICallbackSignatureVerifier
    {
        bool IsValid(
            string rawBody,
            string? signature);
    }
}
