using System;
using System.Collections.Generic;
using System.Text;

namespace OrderingService.Domain.Orders
{
    public class Order
    {
        public string Id { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public Address Address { get; set; }
        public decimal Price { get; set; }
    }
}
