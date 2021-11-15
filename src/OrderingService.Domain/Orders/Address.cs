using System;
using System.Collections.Generic;
using System.Text;

namespace OrderingService.Domain.Orders
{
    public class Address
    {
        public string StreetName { get; set; } 
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(nameof(Address).ToUpperInvariant());
            stringBuilder.Append(nameof(StreetName)).Append("=").Append(StreetName).Append(Environment.NewLine);
            stringBuilder.Append(nameof(City)).Append("=").Append(City).Append(Environment.NewLine);
            stringBuilder.Append(nameof(State)).Append("=").Append(State).Append(Environment.NewLine);
            stringBuilder.Append(nameof(PostalCode)).Append("=").Append(PostalCode).Append(Environment.NewLine);
            return stringBuilder.ToString();
        }
    }
}
