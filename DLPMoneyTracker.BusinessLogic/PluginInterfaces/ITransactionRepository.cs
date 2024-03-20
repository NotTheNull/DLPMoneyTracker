using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.PluginInterfaces
{
    public struct MoneyRecordSearch
    {
        public DateRange DateRange;
        public string SearchText;
        public IJournalAccount? Account;

        public MoneyRecordSearch()
        {
            DateRange = new DateRange(DateTime.MinValue, DateTime.MaxValue);
            SearchText = string.Empty;
            Account = null;
        }
    }

    public interface ITransactionRepository
    {
        List<IMoneyTransaction> Search(MoneyRecordSearch search);
        decimal GetCurrentAccountBalance(Guid accountUID);
        decimal GetAccountBalanceByMonth(Guid accountUID, int year, int month);
        decimal GetAccountBalanceYTD(Guid accountUID, int year);
        void RemoveTransaction(IMoneyTransaction transaction);
        void SaveTransaction(IMoneyTransaction transaction);
    }
}
