using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderingService.Commands.UpdateOrder
{
    public class UpdateOrderCommand : ICommand<Order>
    {
        public string Id { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public Address Address { get; set; }
        public decimal Price { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(nameof(UpdateOrderCommand).ToUpperInvariant());
            stringBuilder.Append(nameof(Id)).Append("=").Append(Id).Append(Environment.NewLine);
            stringBuilder.Append(nameof(CustomerFirstName)).Append("=").Append(CustomerFirstName).Append(Environment.NewLine);
            stringBuilder.Append(nameof(CustomerLastName)).Append("=").Append(CustomerLastName).Append(Environment.NewLine);
            stringBuilder.Append(nameof(Address)).Append("=").Append(Address).Append(Environment.NewLine);
            stringBuilder.Append(nameof(Price)).Append("=").Append(Price).Append(Environment.NewLine);
            return stringBuilder.ToString();
        }
    }
}
