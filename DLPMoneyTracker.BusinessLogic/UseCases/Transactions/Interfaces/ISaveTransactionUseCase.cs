using DLPMoneyTracker.Core.Models;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces
{
    public interface ISaveTransactionUseCase
    {
        void Execute(IMoneyTransaction transaction);
    }
}