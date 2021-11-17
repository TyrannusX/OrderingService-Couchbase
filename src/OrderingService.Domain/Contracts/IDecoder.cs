using System.Threading.Tasks;

namespace OrderingService.Domain.Contracts
{
    public interface IDecoder
    {
        string DomainContentType { get; }

        Task<object> Decode(byte[] item);
    }
}