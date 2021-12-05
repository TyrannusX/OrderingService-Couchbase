using System;
using System.Threading.Tasks;
using Dawn;
using Microsoft.Extensions.Logging;
using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;

namespace OrderingService.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : ICommandHandler<UpdateOrderCommand, Order>
    {
        private readonly ILogger<UpdateOrderCommandHandler> _logger;
        private readonly IRepository<Order> _orderRepository;

        public UpdateOrderCommandHandler(ILogger<UpdateOrderCommandHandler> logger, IRepository<Order> orderRepository)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _orderRepository = Guard.Argument(orderRepository, nameof(orderRepository)).NotNull().Value;
        }

        public async Task<Order> Handle(UpdateOrderCommand command)
        {
            _logger.LogTrace("Handling update order command with command {command}", command);
            Validate(command);
            Order orderToUpdate = new Order();
            orderToUpdate.Id = command.Id;
            orderToUpdate.CustomerFirstName = command.CustomerFirstName;
            orderToUpdate.CustomerLastName = command.CustomerLastName;
            orderToUpdate.Address = command.Address;
            orderToUpdate.Price = command.Price;

            Order order = await _orderRepository.Update(orderToUpdate, command.Id);

            _logger.LogTrace("Updated order successfully for Id {id}", command.Id);

            return order;
        }

        private void Validate(UpdateOrderCommand updateOrderCommand)
        {
            if (updateOrderCommand is null
                && string.IsNullOrWhiteSpace(updateOrderCommand.CustomerFirstName)
                && string.IsNullOrWhiteSpace(updateOrderCommand.CustomerLastName)
                && updateOrderCommand.Address is null
            )
            {
                string message = $"{typeof(UpdateOrderCommand)} is null in {typeof(UpdateOrderCommandHandler)}";
                _logger.LogError(message);
                throw new ArgumentNullException(message);
            }

            if (string.IsNullOrWhiteSpace(updateOrderCommand.Id))
            {
                string message = $"{typeof(UpdateOrderCommand)} Id is null in {typeof(UpdateOrderCommandHandler)}";
                _logger.LogError(message);
                throw new ArgumentNullException(message);
            }
        }
    }
}