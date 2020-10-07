using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using DLPMoneyTracker.DataEntry.BudgetPlanner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DLPMoneyTracker.ReportViews
{
    public class MoneyAccountSummaryVM : BaseViewModel
    {
        private ITrackerConfig _config;
        private IBudgetPlanner _budget;
        private ILedger _ledger;
        private MoneyAccount _act;

        public string AccountID { get { return _act?.ID ?? string.Empty; } }
        public string AccountDesc { get { return _act?.Description ?? string.Empty; } }

        public MoneyAccountType AccountType { get { return _act?.AccountType ?? MoneyAccountType.NotSet; } }

        private decimal _bal;

        public decimal Balance
        {
            get { return _bal; }
            set
            {
                _bal = value;
                NotifyPropertyChanged(nameof(this.Balance));
            }
        }

        public int OrderByAccountType
        {
            get
            {
                switch (AccountType)
                {
                    case MoneyAccountType.Checking: return 1;
                    case MoneyAccountType.CreditCard: return 2;
                    case MoneyAccountType.Savings: return 3;
                    case MoneyAccountType.Loan: return 4;
                    default: return 99;
                }
            }
        }











        ObservableCollection<BudgetRecordVM> _listBudgets = new ObservableCollection<BudgetRecordVM>();
        public ObservableCollection<BudgetRecordVM> BudgetList { get { return _listBudgets; } }

        public bool ShowBudgetData { get { return _listBudgets.Any(); } }

        public decimal BudgetBalance 
        { 
            get
            {
                decimal bal = this.Balance;
                if (!this.BudgetList.Any()) return bal;

                foreach(var budget in this.BudgetList)
                {
                    bal = _ledger.ApplyTransactionToBalance(_act, bal, budget.Category, budget.Amount);
                }

                return bal;
            } 
        }

        #region Commands
        private RelayCommand _cmdDetails;
        public RelayCommand CommandDetails
        {
            get
            {
                return _cmdDetails ?? (_cmdDetails = new RelayCommand((o) =>
                {
                    // TODO: Command Details needs to display the ledger transactions associated with the given account

                }));
            }
        }


        #endregion




        public MoneyAccountSummaryVM(MoneyAccount act, ILedger ledger, IBudgetPlanner budget, ITrackerConfig config)
        {
            _config = config;
            _budget = budget;
            _ledger = ledger;
            _ledger.LedgerModified += () => Refresh();
            _act = act;

            this.Refresh();
        }

        private void LoadBudgets()
        {
            this.BudgetList.Clear();

            var budgetList = _budget.GetUpcomingBudgetListForAccount(this.AccountID);
            if (budgetList is null || !budgetList.Any()) return;

            foreach(var budget in budgetList)
            {
                this.BudgetList.Add(new BudgetRecordVM(_config, budget));
            }
            NotifyPropertyChanged(nameof(this.BudgetBalance));
            NotifyPropertyChanged(nameof(this.ShowBudgetData));
        }



        public void Refresh()
        {
            this.Balance = _ledger.GetAccountBalance(_act);
            this.LoadBudgets();
            this.NotifyAll();
        }

        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.AccountID));
            NotifyPropertyChanged(nameof(this.AccountDesc));
            NotifyPropertyChanged(nameof(this.AccountType));
            NotifyPropertyChanged(nameof(this.Balance));
        }
    }
}
