using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Adapters;
using DLPMoneyTracker.Plugins.SQL.Data;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    public class SQLLedgerAccountRepository(IDLPConfig config) : ILedgerAccountRepository
    {
        private readonly IDLPConfig config = config;

        public IJournalAccount GetAccountByUID(Guid uid)
        {
            using (DataContext context = new(config))
            {
                var account = context.Accounts.FirstOrDefault(x => x.AccountUID == uid);
                if (account is null) return SpecialAccount.InvalidAccount;

                return this.SourceToAccount(account);
            }
        }

        public List<IJournalAccount> GetFullList()
        {
            List<IJournalAccount> listAccountsFinal = new List<IJournalAccount>();
            using (DataContext context = new(config))
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

            using (DataContext context = new(config))
            {
                var listAccountsQuery = context.Accounts.Where(x => search.JournalTypes.Contains(x.AccountType));

                if (!string.IsNullOrWhiteSpace(search.NameFilterText))
                {
                    listAccountsQuery = listAccountsQuery.Where(x => x.Description.Contains(search.NameFilterText));
                }

                if (!search.IncludeDeleted)
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
            using (DataContext context = new(config))
            {
                return context.Accounts.Count();
            }
        }

        public void SaveJournalAccount(IJournalAccount account)
        {
            using (DataContext context = new(config))
            {
                SQLSourceToJournalAccountAdapter adapter = new(this);
                adapter.Copy(account);

                var existingAccount = context.Accounts.FirstOrDefault(x => x.AccountUID == account.Id);
                if (existingAccount is null)
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
            if (type == LedgerType.NotSet) return [];

            List<IJournalAccount> listAccountsFinal = [];
            using (DataContext context = new(config))
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
            if (uidSummaryAccount == Guid.Empty) return [];

            List<IJournalAccount> listAccountsFinal = [];
            using (DataContext context = new(config))
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
            SQLSourceToJournalAccountAdapter adapter = new(this);
            adapter.ImportSource(a);
            return JournalAccountFactory.Build(adapter);
        }

        public Guid GetNextUID()
        {
            Guid next;
            using (DataContext context = new(config))
            {
                do
                {
                    next = Guid.NewGuid();
                } while (DoesUIDExist(next, context));
            }
            return next;
        }

        private bool DoesUIDExist(Guid uid, DataContext context)
        {
            if (uid == Guid.Empty) return true; // Technically it shouldn't exist but we don't want a UID of all zeros
            // Shouldn't need to check for Special Accounts as they should be stored in the database already to not break Foreign Keys

            var id = context.Accounts.FirstOrDefault(x => x.AccountUID == uid)?.Id;
            if (id.HasValue) return true;

            return false;
        }
    }
}