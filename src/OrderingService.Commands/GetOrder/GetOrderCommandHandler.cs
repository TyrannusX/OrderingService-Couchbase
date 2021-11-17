using Dawn;
using Microsoft.Extensions.Logging;
using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Commands.GetOrder
{
    public class GetOrderCommandHandler : ICommandHandler<GetOrderCommand, Order>
    {
        private readonly ILogger<GetOrderCommandHandler> _logger;
        private readonly IRepository<Order> _orderRepository;

        public GetOrderCommandHandler(ILogger<GetOrderCommandHandler> logger, IRepository<Order> orderRepository)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _orderRepository = Guard.Argument(orderRepository, nameof(orderRepository)).NotNull().Value;
        }

        public async Task<Order> Handle(GetOrderCommand command)
        {
            _logger.LogTrace("Attempting to find Order with Id {id}", command.Id);
            Order order = await _orderRepository.Read(command.Id);
            if (order is null)
            {
                string message = $"Order with Id {command.Id} not found";
                _logger.LogError(message);
                throw new Exception(message);
            }
            return order;
        }
    }
}
