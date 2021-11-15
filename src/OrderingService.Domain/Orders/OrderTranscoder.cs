using Newtonsoft.Json.Linq;
using OrderingService.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Domain.Orders
{
    public class OrderTranscoder : ITranscoder<Order>
    {
        public async Task<Order> Decode(byte[] item)
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

        public async Task<byte[]> Encode(Order item)
        {
            JObject orderObject = new JObject();
            orderObject["id"] = item.Id;
            orderObject["customerFirstName"] = item.CustomerFirstName;
            orderObject["customerLastName"] = item.CustomerLastName;

            orderObject["address"] = new JObject();
            orderObject["address"]["streetName"] = item.Address.StreetName;
            orderObject["address"]["city"] = item.Address.City;
            orderObject["address"]["state"] = item.Address.State;
            orderObject["address"]["postalCode"] = item.Address.PostalCode;
            orderObject["price"] = item.Price;

            return Encoding.UTF8.GetBytes(orderObject.ToString());
        }
    }
}
