using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapper.Payments.Domain.Entities
{
    public sealed class SubMerchant
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string ExternalId { get; private set; } = null!;

        public string TradingName { get; private set; } = null!;

        public string City { get; private set; } = null!;

        public string Region { get; private set; } = null!;

        public string Country { get; private set; } = null!;

        public string Mcc { get; private set; } = null!;

        private SubMerchant()
        {
        }

        public SubMerchant(
            string id,
            string name,
            string city,
            string region,
            string country,
            string mcc)
        {
            ExternalId = id;
            TradingName = name;
            City = city;
            Region = region;
            Country = country;
            Mcc = mcc;
        }
    }
}
