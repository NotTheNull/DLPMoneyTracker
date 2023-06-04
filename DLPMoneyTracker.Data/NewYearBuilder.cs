using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels;
using System;

namespace DLPMoneyTracker.Data
{
    public class NewYearBuilder
    {
        // TODO: Rewrite to use new classes
        public static void Execute(int newYear)
        {
            throw new NotImplementedException();
        }
        ///// <summary>
        ///// Copies data from "previous" year to the given year
        ///// </summary>
        ///// <param name="newYear">The year to copy data to</param>
        //public static void Execute(int newYear)
        //{
        //    NewYearBuilder buildIt = new NewYearBuilder(newYear);
        //    buildIt.BuildNewConfig();
        //    buildIt.BuildNewMoneyPlanner();
        //    buildIt.BuildNewBudgetTracker();
        //    buildIt.BuildNewLedger();
        //}

        //private ITrackerConfig _oldConfig, _newConfig;
        //private int _newYear;

        //private NewYearBuilder(int newYear)
        //{
        //    _newYear = newYear;
        //}

        //protected void BuildNewConfig()
        //{
        //    _oldConfig = new TrackerConfig(_newYear - 1);
        //    _newConfig = new TrackerConfig(_newYear);

        //    _newConfig.Copy(_oldConfig);
        //    _newConfig.SaveCategories();
        //    _newConfig.SaveMoneyAccounts();
        //}

        //protected void BuildNewMoneyPlanner()
        //{
        //    MoneyPlanner oldPlanner = new MoneyPlanner(_oldConfig, _newYear - 1);
        //    MoneyPlanner newPlanner = new MoneyPlanner(_newConfig, _newYear);

        //    newPlanner.Copy(oldPlanner);
        //    newPlanner.SaveToFile();
        //}

        //protected void BuildNewBudgetTracker()
        //{
        //    BudgetTracker oldTracker = new BudgetTracker(_oldConfig, _newYear - 1);
        //    BudgetTracker newTracker = new BudgetTracker(_newConfig, _newYear);

        //    newTracker.Copy(oldTracker);
        //    newTracker.SaveToFile();
        //}

        //protected void BuildNewLedger()
        //{
        //    Ledger oldLedger = new Ledger(_oldConfig, _newYear - 1);
        //    Ledger newLedger = new Ledger(_newConfig, _newYear);

        //    foreach (var acct in _newConfig.AccountsList)
        //    {
        //        decimal balance = oldLedger.GetAccountBalance(acct);
        //        newLedger.AddTransaction(new MoneyRecord()
        //        {
        //            Account = acct,
        //            Category = TransactionCategory.InitialBalance,
        //            Description = "*BALANCE FORWARD*",
        //            TransAmount = balance,
        //            TransDate = new DateTime(_newYear, 1, 1)
        //        });
        //    }

        //    newLedger.SaveToFile();
        //}
    }
}