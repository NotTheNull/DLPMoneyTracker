using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.AdapterInterfaces
{
    public interface ISourceToJournalAccountAdapter : IJournalAccount
    {
        void ImportSource(Account acct);
        void ImportCopy(IJournalAccount acct);
        void ExportSource(ref Account acct);
    }
}
