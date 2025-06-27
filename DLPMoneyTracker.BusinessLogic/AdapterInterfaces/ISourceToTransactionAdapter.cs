using DLPMoneyTracker.Core.Models;

namespace DLPMoneyTracker.BusinessLogic.AdapterInterfaces
{
    public interface ISourceToTransactionAdapter<T> : IMoneyTransaction, IDLPAdapter<T>
    {
    }
}