using OrderingService.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderingService.Commands.GetOrder
{
    public class GetOrderCommand : ICommand
    {
        public string Id { get; set; }
    }
}
