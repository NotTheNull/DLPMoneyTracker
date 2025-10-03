using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions;

public class FindTransactionForBudgetPlanUseCase(ITransactionRepository moneyRepository, IDLPConfig config) : IFindTransactionForBudgetPlanUseCase
{
    public IMoneyTransaction? Execute(IBudgetPlan plan, IJournalAccount account)
    {
        // Should only be one record at most but we only need one record as confirmation
        MoneyRecordSearch search = new()
        {
            Account = account,
            SearchText = plan.Description,
            DateRange = new(config.Period.CurrentPayPeriod, config.Period.NextPayPeriod)
        };

        var records = moneyRepository.Search(search);
        if (records?.Any() != true) return null;

        return records[0];
    }
}