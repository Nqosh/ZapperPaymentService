using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Domain.Entities
{
    public sealed class StoredCard
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid CustomerId { get; private set; }

        public Customer Customer { get; private set; } = null!;

        public string AcmeToken { get; private set; } = null!;

        public DateTimeOffset CreatedAt { get; private set; }
            = DateTimeOffset.UtcNow;

        private StoredCard()
        {
        }

        public StoredCard(
            Customer customer,
            string token)
        {
            Customer = customer;
            CustomerId = customer.Id;
            AcmeToken = token;
        }
    }
}
