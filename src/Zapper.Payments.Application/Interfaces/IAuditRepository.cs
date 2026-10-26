using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapper.Payments.Domain.Entities;

namespace Zapper.Payments.Application.Interfaces
{
    public interface IAuditRepository
    {
        void Add(AcmeAttempt attempt);
    }
}
