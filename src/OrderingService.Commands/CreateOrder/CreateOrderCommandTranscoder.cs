using Newtonsoft.Json.Linq;
using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Commands.CreateOrder
{
    public class CreateOrderCommandTranscoder : ITranscoder<CreateOrderCommand>
    {
        public async Task<CreateOrderCommand> Decode(byte[] item)
        {
            //Parse json to JToken
            string json = Encoding.UTF8.GetString(item);
            JToken createOrderObject = JToken.Parse(json);

            //map properties to object
            CreateOrderCommand createOrderCommand = new CreateOrderCommand();
            createOrderCommand.CustomerFirstName = (string)createOrderObject["customerFirstName"];
            createOrderCommand.CustomerLastName = (string)createOrderObject["customerLastName"];
            createOrderCommand.Address = new Address();
            createOrderCommand.Address.StreetName = (string)createOrderObject["address"]["streetName"];
            createOrderCommand.Address.City = (string)createOrderObject["address"]["city"];
            createOrderCommand.Address.State = (string)createOrderObject["address"]["state"];
            createOrderCommand.Address.PostalCode = (string)createOrderObject["address"]["postalCode"];
            createOrderCommand.Price = (decimal)createOrderObject["price"];

            return createOrderCommand;
        }

        public async Task<byte[]> Encode(CreateOrderCommand item)
        {
            JObject createOrderObject = new JObject();
            createOrderObject["customerFirstName"] = item.CustomerFirstName;
            createOrderObject["customerLastName"] = item.CustomerLastName;
            createOrderObject["address"]["streetName"] = item.Address.StreetName;
            createOrderObject["address"]["city"] = item.Address.City;
            createOrderObject["address"]["state"] = item.Address.State;
            createOrderObject["address"]["postalCode"] = item.Address.PostalCode;
            createOrderObject["price"] = item.Price;

            return Encoding.UTF8.GetBytes(createOrderObject.ToString());
        }
    }
}
