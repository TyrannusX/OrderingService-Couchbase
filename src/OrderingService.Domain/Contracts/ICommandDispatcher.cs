using System.Threading.Tasks;

namespace OrderingService.Domain.Contracts
{
    public interface ICommandDispatcher
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand;
        Task<TResult> SendAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
    }
}