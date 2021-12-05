using Dawn;
using Microsoft.Extensions.Logging;
using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, string>
    {
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        private readonly IRepository<Order> _orderRepository;

        public CreateOrderCommandHandler(ILogger<CreateOrderCommandHandler> logger, IRepository<Order> orderRepository)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _orderRepository = Guard.Argument(orderRepository, nameof(orderRepository)).NotNull().Value;
        }

        public async Task<string> Handle(CreateOrderCommand command)
        {
            _logger.LogTrace("Handling create order command with command {command}", command);

            Validate(command);

            string newId = Guid.NewGuid().ToString();
            Order order = new Order
            {
                Id = newId,
                CustomerFirstName = command.CustomerFirstName,
                CustomerLastName = command.CustomerLastName,
                Address = command.Address,
                Price = command.Price
            };

            await _orderRepository.Create(order);

            _logger.LogTrace("Created order successfully. New Id is {id}", newId);

            return newId;
        }

        private void Validate(CreateOrderCommand createOrderCommand)
        {
            if (createOrderCommand is null)
            {
                string message = $"{typeof(CreateOrderCommand)} is null in {typeof(CreateOrderCommandHandler)}";
                _logger.LogError(message);
                throw new ArgumentNullException(message);
            }

            if (string.IsNullOrWhiteSpace(createOrderCommand.CustomerFirstName))
            {
                string message = $"{nameof(createOrderCommand.CustomerFirstName)} is null in {typeof(CreateOrderCommandHandler)}";
                _logger.LogError(message);
                throw new ArgumentNullException(message);
            }

            if (string.IsNullOrWhiteSpace(createOrderCommand.CustomerLastName))
            {
                string message = $"{nameof(createOrderCommand.CustomerLastName)} is null in {typeof(CreateOrderCommandHandler)}";
                _logger.LogError(message);
                throw new ArgumentNullException(message);
            }

            if (createOrderCommand.Address is null)
            {
                string message = $"{nameof(createOrderCommand.Address)} is null in {typeof(CreateOrderCommandHandler)}";
                _logger.LogError(message);
                throw new ArgumentNullException(message);
            }
        }
    }
}
