using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public enum LedgerTypes
    {
        Bank,
        LiabilityCard,
        LiabilityLoan,
        Receivable,
        Payable
    }

    public interface ILedgerAccount
    {
        Guid Id { get; }
        string Description { get; set; }
        LedgerTypes LedgerType { get; }
        
        DateTime? DateClosedUTC { get; set; }
    }
}
