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



    }
}
