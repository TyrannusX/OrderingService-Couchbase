using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Domain.Contracts
{
    public interface ITranscoder<T> where T: class
    {
        Task<T> Decode(byte[] item);
        Task<byte[]> Encode(T item);
    }
}
