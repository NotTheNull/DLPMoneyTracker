using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class FindTransactionForBudgetPlanUseCase(ITransactionRepository moneyRepository) : IFindTransactionForBudgetPlanUseCase
    {
        public IMoneyTransaction? Execute(IBudgetPlan plan, IJournalAccount account)
        {
            // Should only be one record at most but we only need one record as confirmation
            MoneyRecordSearch search = new()
            {
                Account = account,
                SearchText = plan.Description,
                DateRange = new Core.DateRange(plan.NotificationDate, plan.NextOccurrence.AddDays(5))
            };

            var records = moneyRepository.Search(search);
            if (records?.Any() != true) return null;

            return records[0];
        }
    }
}