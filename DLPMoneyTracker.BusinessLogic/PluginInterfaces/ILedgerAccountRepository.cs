using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.PluginInterfaces
{
    public struct JournalAccountSearch
    {
        public List<LedgerType> JournalTypes;
        public string NameFilterText;
        public bool IncludeDeleted;

        public static JournalAccountSearch GetAccountsByType(List<LedgerType> listTypes)
        {
            return new JournalAccountSearch
            {
                JournalTypes = listTypes,
                IncludeDeleted = false,
                NameFilterText = string.Empty
            };
        }

        public static JournalAccountSearch GetMoneyAccounts(bool includDeleted = false)
        {
            return new JournalAccountSearch
            {
                JournalTypes = new List<LedgerType> { LedgerType.Bank, LedgerType.LiabilityCard },
                IncludeDeleted = includDeleted,
                NameFilterText = string.Empty
            };
        }


        public static JournalAccountSearch GetLedgerAccounts(bool includeDeleted = false)
        {
            return new JournalAccountSearch
            {
                JournalTypes = new List<LedgerType>() { LedgerType.Payable, LedgerType.Receivable },
                IncludeDeleted = includeDeleted,
                NameFilterText = string.Empty
            };
        }

    }

    public interface ILedgerAccountRepository
    {

        IJournalAccount GetAccountByUID(Guid uid);
        List<IJournalAccount> GetAccountsBySearch(JournalAccountSearch search);
        void SaveJournalAccount(IJournalAccount account);
    }
}
