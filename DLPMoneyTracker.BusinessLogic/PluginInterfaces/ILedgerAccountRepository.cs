﻿using DLPMoneyTracker.Core.Models.LedgerAccounts;

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

        public static JournalAccountSearch GetMoneyAccounts(bool includeDeleted = false)
        {
            return new JournalAccountSearch
            {
                JournalTypes = [LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan],
                IncludeDeleted = includeDeleted,
                NameFilterText = string.Empty
            };
        }

        public static JournalAccountSearch GetPaymentAccounts(bool includeDeleted = false)
        {
            return new JournalAccountSearch
            {
                JournalTypes = [LedgerType.Bank, LedgerType.LiabilityCard],
                IncludeDeleted = includeDeleted,
                NameFilterText = string.Empty
            };
        }

        public static JournalAccountSearch GetNominalAccounts(bool includeDeleted = false)
        {
            return new JournalAccountSearch
            {
                JournalTypes = [LedgerType.Payable, LedgerType.Receivable],
                IncludeDeleted = includeDeleted,
                NameFilterText = string.Empty
            };
        }
    }

    public interface ILedgerAccountRepository
    {
        IJournalAccount GetAccountByUID(Guid uid);

        List<IJournalAccount> GetFullList();

        List<IJournalAccount> GetAccountsBySearch(JournalAccountSearch search);

        List<IJournalAccount> GetSummaryAccountListByType(LedgerType type);

        List<IJournalAccount> GetDetailAccountsForSummary(Guid uidSummaryAccount);

        void SaveJournalAccount(IJournalAccount account);

        int GetRecordCount();

        Guid GetNextUID();
    }
}