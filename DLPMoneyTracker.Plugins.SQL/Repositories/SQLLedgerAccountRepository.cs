using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
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
            throw new NotImplementedException();
        }
    }
}
