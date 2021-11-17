using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Domain.Contracts
{
    public interface ITranscoder
    {
        Task<object> Decode(byte[] item, string contentType);
        Task<byte[]> Encode(object item, string contentType);
    }
}
