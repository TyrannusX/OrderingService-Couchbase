using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderingService.Commands.DeleteOrder
{
    public class DeleteOrderCommand : ICommand
    {
        public string Id { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(nameof(DeleteOrderCommand).ToUpperInvariant());
            stringBuilder.Append(nameof(Id)).Append("=").Append(Id).Append(Environment.NewLine);
            return stringBuilder.ToString();
        }
    }
}
