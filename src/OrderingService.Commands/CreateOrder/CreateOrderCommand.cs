using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderingService.Commands.CreateOrder
{
    public class CreateOrderCommand : ICommand
    {
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public Address Address { get; set; }
        public decimal Price { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(nameof(CreateOrderCommand).ToUpperInvariant());
            stringBuilder.Append(nameof(CustomerFirstName)).Append("=").Append(CustomerFirstName).Append(Environment.NewLine);
            stringBuilder.Append(nameof(CustomerLastName)).Append("=").Append(CustomerLastName).Append(Environment.NewLine);
            stringBuilder.Append(nameof(Address)).Append("=").Append(Address).Append(Environment.NewLine);
            stringBuilder.Append(nameof(Price)).Append("=").Append(Price).Append(Environment.NewLine);
            return stringBuilder.ToString();
        }
    }
}
