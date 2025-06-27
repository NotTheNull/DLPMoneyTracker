using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation
{
    public class GetReconciliationTransactionsUseCase(IBankReconciliationRepository reconciliationRepository) : IGetReconciliationTransactionsUseCase
    {
        public List<IMoneyTransaction> Execute(Guid accountUID, DateRange statementDates)
        {
            return reconciliationRepository.GetReconciliationTransactions(accountUID, statementDates);
        }
    }
}