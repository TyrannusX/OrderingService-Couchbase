using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Domain.Contracts
{
    public interface IRepository<T> where T : class
    {
        Task<string> Create(T item);
        Task<T> Read(string id);
        Task<T> Update(T item, string id);
        Task Delete(string id);
    }
}
