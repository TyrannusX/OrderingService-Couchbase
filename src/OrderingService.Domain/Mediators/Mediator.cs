using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.Extensions.Logging;
using OrderingService.Domain.Contracts;

namespace OrderingService.Domain.Mediators
{
    public class Mediator : IMediator
    {
        private readonly ILogger<Mediator> _logger;
        private readonly IEnumerable<ICommandHandler<ICommand, Type>> _commandHandlers;

        public Mediator(ILogger<Mediator> logger, IEnumerable<ICommandHandler<ICommand, Type>> commandHandlers)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _commandHandlers = Guard.Argument(commandHandlers, nameof(commandHandlers)).NotNull().NotEmpty().Value;
        }

        public async Task<object> Send(ICommand command)
        {
            if (command is not ICommand inputCommand)
            {
                string message = $"{command} is not of type {typeof(ICommand)}";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            ICommandHandler<ICommand, Type> commandHandler = _commandHandlers.FirstOrDefault(handler =>
            {
                Type handlerType = handler.GetType();
                Type[] generics = handlerType.GetGenericArguments();
                Type inputCommandType = inputCommand.GetType();

                return inputCommandType == generics[0];
            });

            return commandHandler.Handle(inputCommand);
        }
    }
}