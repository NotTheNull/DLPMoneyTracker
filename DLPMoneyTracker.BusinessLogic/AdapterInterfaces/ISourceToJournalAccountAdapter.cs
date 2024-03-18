using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.AdapterInterfaces
{
    // T is the Source class
    public interface ISourceToJournalAccountAdapter<T> : IJournalAccount, IDLPAdapter<T>
    {
        
    }
}
