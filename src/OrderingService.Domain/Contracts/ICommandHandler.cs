using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Domain.Contracts
{
    public interface ICommandHandler<TStart, TResult> where TStart : ICommand
    {
        Task<TResult> Handle(TStart command);
    }
}
