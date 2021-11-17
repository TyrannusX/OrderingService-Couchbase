using System.Threading.Tasks;

namespace OrderingService.Domain.Contracts
{
    public interface IEncoder
    {
        string DomainContentType { get; }

        Task<byte[]> Encode(object item);
    }
}