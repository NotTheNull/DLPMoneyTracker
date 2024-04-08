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

    public enum BudgetTrackingType
    {
        DO_NOT_TRACK,
        Fixed, // Expense (Utility Bill) or Income (Paycheck)
        Variable // Expense (Grocery) or Income (Roommates)
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
        BudgetTrackingType BudgetType { get; }
        decimal DefaultMonthlyBudgetAmount { get; }
        decimal CurrentBudgetAmount { get; }
    }


    #region FUNDS IDEA
    /*
     * The idea was to have a system similar to "Envelopes of Cash" i.e. set aside money for a specific purpose
     * MY problem is that I'm too far behind;  all of my expenses go onto the credit cards but the total debt exceeds my paychecks
     * 
     * Leaving this reminder here in case I ever want to revisit
     * Will need to strip out the Ledger #s and Category Ids
     * 
     */



    //// This is a sub-ledger of a "Bank" account that holds an allocation of money for a particular Expense
    //// The point is for this to serve as a means of Budgeting by filtering my paycheck into various accounts with intended purpose
    //public interface IFundAccount : IJournalAccount
    //{

    //}


    //// This is a sub-ledger of a "Payable" account that is used to help balance the Fund accounts
    //public interface IExpenseTrackingAccount : IJournalAccount
    //{
    //    IMoneyAccount MoneyAccount { get; } // This is the money account the expense is reported with
    //}
    #endregion

}
