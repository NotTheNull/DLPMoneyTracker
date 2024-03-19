using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces
{
    public interface IGetReconciliationTransactionsUseCase
    {
        List<IMoneyTransaction> Execute(Guid accountUID, DateRange statementDates);
    }
}