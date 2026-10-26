using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Domain.Entities
{
    public sealed class Customer
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string ExternalId { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        public List<StoredCard> StoredCards { get; private set; } = [];

        private Customer()
        {
        }

        public Customer(
            string externalId,
            string email)
        {
            ExternalId = externalId;
            Email = email;
        }
    }
}
