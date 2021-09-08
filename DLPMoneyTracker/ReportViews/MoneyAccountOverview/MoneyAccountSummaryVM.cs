using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using DLPMoneyTracker.DataEntry.AddTransaction;
using DLPMoneyTracker.DataEntry.BudgetPlanner;
using DLPMoneyTracker.ReportViews.LedgerViews;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DLPMoneyTracker.ReportViews
{
    public class MoneyAccountSummaryVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly IMoneyPlanner _budget;
        private readonly ILedger _ledger;
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

        private ObservableCollection<MoneyPlanRecordVM> _listBudgets = new ObservableCollection<MoneyPlanRecordVM>();
        public ObservableCollection<MoneyPlanRecordVM> MoneyPlanList { get { return _listBudgets; } }

        public bool ShowBudgetData { get { return _listBudgets.Any(); } }

        public decimal BudgetBalance
        {
            get
            {
                decimal bal = this.Balance;
                if (!this.MoneyPlanList.Any()) return bal;

                foreach (var budget in this.MoneyPlanList)
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
                    LedgerDetailView uiAccountLedger = UICore.DependencyHost.GetService<LedgerDetailView>();
                    uiAccountLedger.ShowAccountDetail(_act);
                    Window windowLedger = new Window()
                    {
                        Content = uiAccountLedger,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Title = "Ledger Detail",
                        Width = 750,
                        Height = 500
                    };
                    windowLedger.Show();
                }));
            }
        }

        private RelayCommand _cmdCreateTransaction;

        public RelayCommand CommandCreateTransaction
        {
            get
            {
                return _cmdCreateTransaction ?? (_cmdCreateTransaction = new RelayCommand((o) =>
                {
                    if (o is MoneyPlanRecordVM plan)
                    {
                        this.CreateTransaction(plan.GetSource());
                    }
                }));
            }
        }

        #endregion Commands

        public MoneyAccountSummaryVM(MoneyAccount act, ILedger ledger, IMoneyPlanner budget, ITrackerConfig config)
        {
            _config = config;
            _budget = budget;
            _ledger = ledger;
            _ledger.LedgerModified += () => Refresh();
            _act = act;

            this.Refresh();
        }

        private void LoadMoneyPlan()
        {
            this.MoneyPlanList.Clear();

            var moneyList = _budget.GetUpcomingMoneyPlansForAccount(this.AccountID);
            if (moneyList is null || !moneyList.Any()) return;

            foreach (var budget in moneyList.OrderBy(o => o.NotificationDate).ThenBy(o => o.PriorityOrder))
            {
                // Check to see if there's a ledger record that matches; if so, skip the money plan so we're not misled
                if (budget.CategoryName.Contains("Transfer"))
                {
                    // Transfers should be handled especially
                    if (_ledger.TransactionList.Any(x =>
                        (
                            x.CategoryUID == TransactionCategory.TransferTo.ID
                            || x.CategoryUID == TransactionCategory.TransferFrom.ID
                        )
                        && x.AccountID == budget.AccountID
                        && x.TransDate >= budget.NotificationDate))
                    {
                        continue;
                    }
                }
                else if (budget.CategoryName.Contains("Debt"))
                {
                    // Debt payments also have to be handled especially
                    if (_ledger.TransactionList.Any(x => x.CategoryUID == TransactionCategory.DebtPayment.ID && x.AccountID == budget.AccountID && x.TransDate > budget.NotificationDate))
                    {
                        continue;
                    }
                }
                else
                {
                    if (_ledger.TransactionList.Any(x => x.CategoryUID == budget.CategoryID && x.Description == budget.Description && x.TransDate >= budget.NotificationDate))
                    {
                        continue;
                    }
                }

                this.MoneyPlanList.Add(new MoneyPlanRecordVM(_config, budget));
            }
            NotifyPropertyChanged(nameof(this.BudgetBalance));
            NotifyPropertyChanged(nameof(this.ShowBudgetData));
        }

        public void CreateTransaction(IMoneyPlan plan)
        {
            if (plan is ExpensePlan expense)
            {
                AddExpenseView uiAddExpense = UICore.DependencyHost.GetService<AddExpenseView>();
                uiAddExpense.CreateTransactionFromMoneyPlan(expense);
                uiAddExpense.Show();
            }
            else if (plan is IncomePlan income)
            {
                AddIncomeView uiAddIncome = UICore.DependencyHost.GetService<AddIncomeView>();
                uiAddIncome.CreateTransactionFromMoneyPlan(income);
                uiAddIncome.Show();
            }
        }

        public void Refresh()
        {
            this.Balance = _ledger.GetAccountBalance(_act);
            this.LoadMoneyPlan();
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