using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OrderingService.Domain.Contracts;

namespace OrderingService.Commands.UpdateOrder
{
    public class UpdateOrderCommandEncoder : IEncoder
    {
        public string DomainContentType { get; } = $"{nameof(UpdateOrderCommand)}";

        public async Task<byte[]> Encode(object item)
        {
            if (item is not UpdateOrderCommand updateOrderCommand)
            {
                throw new InvalidOperationException($"Item is not of type {typeof(UpdateOrderCommand)}");
            }

            JObject updateOrderObject = new JObject();
            updateOrderObject["id"] = updateOrderCommand.Id;
            updateOrderObject["customerFirstName"] = updateOrderCommand.CustomerFirstName;
            updateOrderObject["customerLastName"] = updateOrderCommand.CustomerLastName;
            updateOrderObject["address"]["streetName"] = updateOrderCommand.Address.StreetName;
            updateOrderObject["address"]["city"] = updateOrderCommand.Address.City;
            updateOrderObject["address"]["state"] = updateOrderCommand.Address.State;
            updateOrderObject["address"]["postalCode"] = updateOrderCommand.Address.PostalCode;
            updateOrderObject["price"] = updateOrderCommand.Price;

            return Encoding.UTF8.GetBytes(updateOrderObject.ToString());
        }
    }
}