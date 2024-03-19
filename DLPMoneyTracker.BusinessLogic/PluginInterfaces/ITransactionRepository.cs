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
        public DateRange? DateRange;
        public string? SearchText;
        public IJournalAccount? Account;
    }

    public interface ITransactionRepository
    {
        List<IMoneyTransaction> Search(MoneyRecordSearch search);
        decimal GetAccountBalance(Guid uid);
        void GetTransactionsForAccount(Guid accountUID);
        void RemoveTransaction(IMoneyTransaction transaction);
        void SaveTransaction(IMoneyTransaction transaction);
    }
}
