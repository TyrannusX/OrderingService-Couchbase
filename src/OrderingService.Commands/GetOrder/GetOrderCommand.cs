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
    }
}
