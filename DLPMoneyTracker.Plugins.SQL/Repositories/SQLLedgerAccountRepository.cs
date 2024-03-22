using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    public class SQLLedgerAccountRepository : ILedgerAccountRepository
    {
        public IJournalAccount GetAccountByUID(Guid uid)
        {
            using(DataContext context = new DataContext())
            {
                var account = context.Accounts.FirstOrDefault(x => x.AccountUID == uid);
                if (account is null) return null;

                SQLSourceToJournalAccountAdapter adapter = new SQLSourceToJournalAccountAdapter();
                adapter.ImportSource(account);

                JournalAccountFactory factory = new JournalAccountFactory();
                return factory.Build(adapter);
            }
        }

        public List<IJournalAccount> GetFullList()
        {
            List<IJournalAccount> listAccountsFinal = new List<IJournalAccount>();
            using (DataContext context = new DataContext())
            {
                var listAccounts = context.Accounts.ToList();
                if (listAccounts?.Any() != true) return listAccountsFinal;

                SQLSourceToJournalAccountAdapter adapter = new SQLSourceToJournalAccountAdapter();
                JournalAccountFactory factory = new JournalAccountFactory();
                foreach (var account in listAccounts)
                {
                    adapter.ImportSource(account);
                    listAccountsFinal.Add(factory.Build(adapter));
                }
            }

            return listAccountsFinal;
        }

        public List<IJournalAccount> GetAccountsBySearch(JournalAccountSearch search)
        {
            List<IJournalAccount> listAccountsFinal = new List<IJournalAccount>();
            if (search.JournalTypes?.Any() != true) return listAccountsFinal;

            using (DataContext context = new DataContext())
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
                                
                SQLSourceToJournalAccountAdapter adapter = new SQLSourceToJournalAccountAdapter();
                JournalAccountFactory factory = new JournalAccountFactory();
                foreach(var account in listAccountsLoop) 
                {
                    adapter.ImportSource(account);
                    listAccountsFinal.Add(factory.Build(adapter));
                }
            }

            return listAccountsFinal;
        }

        public int GetRecordCount()
        {
            using (DataContext context = new DataContext())
            {
                return context.Accounts.Count();
            }
        }

        public void SaveJournalAccount(IJournalAccount account)
        {
            using (DataContext context = new DataContext())
            {
                SQLSourceToJournalAccountAdapter adapter = new SQLSourceToJournalAccountAdapter();
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
    }
}
