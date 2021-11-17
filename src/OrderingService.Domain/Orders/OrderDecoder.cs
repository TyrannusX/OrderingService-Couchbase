using Newtonsoft.Json.Linq;
using OrderingService.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Domain.Orders
{
    public class OrderDecoder : IDecoder
    {
        public string DomainContentType { get; } = $"{nameof(Order)}";

        public async Task<object> Decode(byte[] item)
        {
            //Parse json to JToken
            string json = Encoding.UTF8.GetString(item);
            JToken orderObject = JToken.Parse(json);

            //map properties to object
            Order order = new Order();
            order.Id = (string)orderObject["id"];
            order.CustomerFirstName = (string)orderObject["customerFirstName"];
            order.CustomerLastName = (string)orderObject["customerLastName"];
            order.Address = new Address();
            order.Address.StreetName = (string)orderObject["address"]["streetName"];
            order.Address.City = (string)orderObject["address"]["city"];
            order.Address.State = (string)orderObject["address"]["state"];
            order.Address.PostalCode = (string)orderObject["address"]["postalCode"];
            order.Price = (decimal)orderObject["price"];

            return order;
        }
    }
}
