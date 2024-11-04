using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Adapters;
using DLPMoneyTracker.Plugins.SQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    public class SQLLedgerAccountRepository : ILedgerAccountRepository
    {
        private readonly IDLPConfig config;

        public SQLLedgerAccountRepository(IDLPConfig config)
        {
            this.config = config;
        }

        public IJournalAccount GetAccountByUID(Guid uid)
        {
            using(DataContext context = new DataContext(config))
            {
                var account = context.Accounts.FirstOrDefault(x => x.AccountUID == uid);
                if (account is null) return null;

                return this.SourceToAccount(account);
            }
        }

        public List<IJournalAccount> GetFullList()
        {
            List<IJournalAccount> listAccountsFinal = new List<IJournalAccount>();
            using (DataContext context = new DataContext(config))
            {
                var listAccounts = context.Accounts.ToList();
                if (listAccounts?.Any() != true) return listAccountsFinal;

                foreach (var account in listAccounts)
                {
                    listAccountsFinal.Add(this.SourceToAccount(account));
                }
            }

            return listAccountsFinal;
        }

        public List<IJournalAccount> GetAccountsBySearch(JournalAccountSearch search)
        {
            List<IJournalAccount> listAccountsFinal = new List<IJournalAccount>();
            if (search.JournalTypes?.Any() != true) return listAccountsFinal;

            using (DataContext context = new DataContext(config))
            {
                var listAccountsQuery = context.Accounts.Where(x => search.JournalTypes.Contains(x.AccountType));

                if(!string.IsNullOrWhiteSpace(search.NameFilterText))
                {
                    listAccountsQuery = listAccountsQuery.Where(x => x.Description.Contains(search.NameFilterText));
                }

                if(!search.IncludeDeleted)
                {
                    listAccountsQuery = listAccountsQuery.Where(x => !x.DateClosedUTC.HasValue);
                }

                var listAccountsLoop = listAccountsQuery.ToList();
                if (listAccountsLoop?.Any() != true) return listAccountsFinal;

                foreach (var account in listAccountsLoop)
                {
                    listAccountsFinal.Add(this.SourceToAccount(account));
                }
            }

            return listAccountsFinal;
        }

        public int GetRecordCount()
        {
            using (DataContext context = new DataContext(config))
            {
                return context.Accounts.Count();
            }
        }

        public void SaveJournalAccount(IJournalAccount account)
        {
            using (DataContext context = new DataContext(config))
            {
                SQLSourceToJournalAccountAdapter adapter = new SQLSourceToJournalAccountAdapter(this);
                adapter.Copy(account);

                var existingAccount = context.Accounts.FirstOrDefault(x => x.AccountUID == account.Id);
                if(existingAccount is null)
                {
                    existingAccount = new Data.Account();
                    context.Accounts.Add(existingAccount);
                }

                adapter.ExportSource(ref existingAccount);
                context.SaveChanges();
            }
        }

        public List<IJournalAccount> GetSummaryAccountListByType(LedgerType type)
        {
            if (type == LedgerType.NotSet) return null;

            List<IJournalAccount> listAccountsFinal = new List<IJournalAccount>();
            using (DataContext context = new DataContext(config))
            {
                var listAccountsLoop = context.Accounts.Where(x => x.AccountType == type && x.SummaryAccountId == null).ToList();
                foreach (var account in listAccountsLoop)
                {
                    listAccountsFinal.Add(this.SourceToAccount(account));
                }
            }

            return listAccountsFinal;
        }

        public List<IJournalAccount> GetDetailAccountsForSummary(Guid uidSummaryAccount)
        {
            if (uidSummaryAccount == null || uidSummaryAccount == Guid.Empty) return null;

            List<IJournalAccount> listAccountsFinal = new List<IJournalAccount>();
            using (DataContext context = new DataContext(config))
            {
                var listAccountsLoop = context.Accounts.Where(x => x.SummaryAccount != null && x.SummaryAccount.AccountUID == uidSummaryAccount).ToList();
                foreach (var account in listAccountsLoop)
                {
                    listAccountsFinal.Add(this.SourceToAccount(account));
                }
            }

            return listAccountsFinal;
        }



        private IJournalAccount SourceToAccount(Account a)
        {
            SQLSourceToJournalAccountAdapter adapter = new SQLSourceToJournalAccountAdapter(this);
            adapter.ImportSource(a);

            JournalAccountFactory factory = new JournalAccountFactory();
            return factory.Build(adapter);
        }
    }
}
