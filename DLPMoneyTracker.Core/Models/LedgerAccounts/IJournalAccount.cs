using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.LedgerAccounts
{
    public enum LedgerType
    {
        Bank,
        LiabilityCard,
        LiabilityLoan,
        Receivable,
        Payable,
        NotSet
    }


    public interface IJournalAccount
    {
        Guid Id { get; }
        string Description { get; }
        LedgerType JournalType { get; }
        int OrderBy { get; }
        DateTime? DateClosedUTC { get; set; }


        void Copy(IJournalAccount cpy);
    }

    public interface IMoneyAccount : IJournalAccount
    {

    }

    public interface ILiabilityAccount : IJournalAccount
    {
        
    }

    public interface INominalAccount : IJournalAccount
    {
        // Controls whether the Income's / Expense's subtotal is displayed on the account's Card UI
        public bool IsTracked { get; set; }
    }

    

}
