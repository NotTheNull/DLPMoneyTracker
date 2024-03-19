using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class FindTransactionForBudgetPlanUseCase : IFindTransactionForBudgetPlanUseCase
    {
        private readonly ITransactionRepository moneyRepository;

        public FindTransactionForBudgetPlanUseCase(ITransactionRepository moneyRepository)
        {
            this.moneyRepository = moneyRepository;
        }

        public IMoneyTransaction Execute(IBudgetPlan plan, IJournalAccount account)
        {
            // Should only be one record at most but we only need one record as confirmation
            MoneyRecordSearch search = new MoneyRecordSearch()
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
