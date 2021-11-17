using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OrderingService.Domain.Contracts;

namespace OrderingService.Commands.CreateOrder
{
    public class CreateOrderCommandEncoder : IEncoder
    {
        public string DomainContentType { get; } = $"{nameof(CreateOrderCommand)}";

        public async Task<byte[]> Encode(object item)
        {
            if (item is not CreateOrderCommand createOrderCommand)
            {
                throw new InvalidOperationException($"Item is not of type {typeof(CreateOrderCommand)}");
            }

            JObject createOrderObject = new JObject();
            createOrderObject["customerFirstName"] = createOrderCommand.CustomerFirstName;
            createOrderObject["customerLastName"] = createOrderCommand.CustomerLastName;
            createOrderObject["address"]["streetName"] = createOrderCommand.Address.StreetName;
            createOrderObject["address"]["city"] = createOrderCommand.Address.City;
            createOrderObject["address"]["state"] = createOrderCommand.Address.State;
            createOrderObject["address"]["postalCode"] = createOrderCommand.Address.PostalCode;
            createOrderObject["price"] = createOrderCommand.Price;

            return Encoding.UTF8.GetBytes(createOrderObject.ToString());
        }
    }
}