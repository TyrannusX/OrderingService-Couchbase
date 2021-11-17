using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OrderingService.Domain.Contracts;

namespace OrderingService.Domain.Orders
{
    public class OrderEncoder : IEncoder
    {
        public string DomainContentType { get; } = $"{nameof(Order)}";

        public async Task<byte[]> Encode(object item)
        {
            if (item is not Order order)
            {
                throw new InvalidOperationException($"Item is not of type {typeof(Order)}");
            }

            JObject orderObject = new JObject();
            orderObject["id"] = order.Id;
            orderObject["customerFirstName"] = order.CustomerFirstName;
            orderObject["customerLastName"] = order.CustomerLastName;

            orderObject["address"] = new JObject();
            orderObject["address"]["streetName"] = order.Address.StreetName;
            orderObject["address"]["city"] = order.Address.City;
            orderObject["address"]["state"] = order.Address.State;
            orderObject["address"]["postalCode"] = order.Address.PostalCode;
            orderObject["price"] = order.Price;

            return Encoding.UTF8.GetBytes(orderObject.ToString());
        }
    }
}