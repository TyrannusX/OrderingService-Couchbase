using Newtonsoft.Json.Linq;
using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Commands.UpdateOrder
{
    public class UpdateOrderCommandDecoder : IDecoder
    {
        public string DomainContentType { get; } = $"{nameof(UpdateOrderCommand)}";

        public async Task<object> Decode(byte[] item)
        {
            //Parse json to JToken
            string json = Encoding.UTF8.GetString(item);
            JToken updateOrderObject = JToken.Parse(json);

            //map properties to object
            UpdateOrderCommand updateOrderCommand = new UpdateOrderCommand();
            updateOrderCommand.Id = (string)updateOrderObject["id"];
            updateOrderCommand.CustomerFirstName = (string)updateOrderObject["customerFirstName"];
            updateOrderCommand.CustomerLastName = (string)updateOrderObject["customerLastName"];
            updateOrderCommand.Address = new Address();
            updateOrderCommand.Address.StreetName = (string)updateOrderObject["address"]["streetName"];
            updateOrderCommand.Address.City = (string)updateOrderObject["address"]["city"];
            updateOrderCommand.Address.State = (string)updateOrderObject["address"]["state"];
            updateOrderCommand.Address.PostalCode = (string)updateOrderObject["address"]["postalCode"];
            updateOrderCommand.Price = (decimal)updateOrderObject["price"];

            return updateOrderCommand;
        }
    }
}
