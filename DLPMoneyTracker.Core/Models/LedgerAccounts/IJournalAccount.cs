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

    // TODO: Add some General Ledger style ids to help group these together
    public interface IJournalAccount
    {
        Guid Id { get; }
        string Description { get; }
        LedgerType JournalType { get; }
        string LedgerNumber { get; } // Combination of Account Type #, Category #, Subledger #
        int CategoryId { get; }
        int SubLedgerId { get; } // 0 = is always the main account; all others will be a Fund type 


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

    }

    // Funds serve as sub-ledgers for Money Accounts 

    // These are Income / Expense Accounts linked to a Money Account; they total into the main Receivable / Payable account
    // Incomes should only be linked to a Bank account
    // Expenses can be any money account
    public interface IFundAccount : IJournalAccount
    {
        IMoneyAccount BankAccount { get; } // This is the account the funds are paired with
    }


}
