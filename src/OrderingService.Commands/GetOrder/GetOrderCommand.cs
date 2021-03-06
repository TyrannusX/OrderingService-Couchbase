using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderingService.Commands.GetOrder
{
    public class GetOrderCommand : ICommand<Order>
    {
        public string Id { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(nameof(GetOrderCommand).ToUpperInvariant());
            stringBuilder.Append(nameof(Id)).Append("=").Append(Id).Append(Environment.NewLine);
            return stringBuilder.ToString();
        }
    }
}
