using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Application.Contracts
{
    public sealed record ServiceResult<T>(
       T? Value,
       string? Error,
       bool Created = false)
    {
        public bool Success => Error is null;
    }
}
