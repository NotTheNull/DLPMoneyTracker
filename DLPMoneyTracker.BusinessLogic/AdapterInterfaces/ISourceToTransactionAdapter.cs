using DLPMoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.AdapterInterfaces
{
    public interface ISourceToTransactionAdapter<T> : IMoneyTransaction, IDLPAdapter<T>
    {
    }
}
