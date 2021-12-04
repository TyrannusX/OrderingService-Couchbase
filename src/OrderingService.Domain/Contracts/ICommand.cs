using System;
using System.Collections.Generic;
using System.Text;

namespace OrderingService.Domain.Contracts
{
    public interface ICommand
    {

    }

    public interface ICommand<out TResult>
    {

    }
}
