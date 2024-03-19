using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces
{
    public interface IFindTransactionForBudgetPlanUseCase
    {
        IMoneyTransaction Execute(IBudgetPlan plan, IJournalAccount account);
    }
}