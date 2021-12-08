using System;
using System.Threading.Tasks;
using Dawn;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderingService.Domain.Contracts;

namespace OrderingService.Infrastructure.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IServiceScopeFactory serviceScopeFactory, ILogger<CommandDispatcher> logger)
        {
            _serviceScopeFactory = Guard.Argument(serviceScopeFactory, nameof(serviceScopeFactory)).NotNull().Value;
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
        }

        public async Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            try
            {
                ICommandHandler<TCommand> matchingCommandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();
                await matchingCommandHandler.Handle(command);
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogError("Could not resolve a command handler for command {command}: {ex}", command.GetType(), ex);
                throw;
            }
        }

        public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            
            try
            {
                ICommandHandler<TCommand, TResult> matchingCommandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
                return await matchingCommandHandler.Handle(command);
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogError("Could not resolve a command handler for command {command}: {ex}", command.GetType(), ex);
                throw;
            }
        }
    }
}