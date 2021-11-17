using System.Threading.Tasks;

namespace OrderingService.Domain.Contracts
{
    public interface IMediator
    {
        Task<object> Send(ICommand command);
    }
}