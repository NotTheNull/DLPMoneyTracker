using DLPMoneyTracker.Core.Models;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces
{
    public interface IGetTransactionByIdUseCase
    {
        IMoneyTransaction Execute(Guid uid);
    }
}