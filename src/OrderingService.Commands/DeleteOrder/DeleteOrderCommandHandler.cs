using Dawn;
using Microsoft.Extensions.Logging;
using OrderingService.Commands.DeleteOrder;
using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : ICommandHandler<DeleteOrderCommand>
    {
        private readonly ILogger<DeleteOrderCommandHandler> _logger;
        private readonly IRepository<Order> _orderRepository;

        public DeleteOrderCommandHandler(ILogger<DeleteOrderCommandHandler> logger, IRepository<Order> orderRepository)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _orderRepository = Guard.Argument(orderRepository, nameof(orderRepository)).NotNull().Value;
        }

        public async Task Handle(DeleteOrderCommand command)
        {
            _logger.LogTrace("Handling delete order command with data {data}", command);
            _logger.LogTrace("Attempting to find Order with Id {id} to delete", command.Id);
            Order order = await _orderRepository.Read(command.Id);
            if (order is null)
            {
                string message = $"Order with Id {command.Id} not found";
                _logger.LogError(message);
                throw new Exception(message);
            }
            await _orderRepository.Delete(command.Id);
            _logger.LogTrace("Successfully deleted order with id {id}", command.Id);
        }
    }
}
