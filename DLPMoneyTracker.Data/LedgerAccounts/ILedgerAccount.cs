using DLPMoneyTracker.Data.ConfigModels;
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
        Payable,
        NotSet
    }

    public interface ILedgerAccount
    {
        Guid Id { get; }
        string Description { get; }
        LedgerTypes LedgerType { get;}
        int OrderBy { get; }
        DateTime? DateClosedUTC { get; }

        string MoneyAccountId { get; }
        MoneyAccountType AccountType { get; }
        Guid CategoryId { get; } // Reference to legacy TransactionCategory

        void Copy(ILedgerAccount cpy);
    }
}
