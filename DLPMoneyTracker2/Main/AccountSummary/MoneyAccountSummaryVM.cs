using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker2.Main.TransactionList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.AccountSummary
{
    public class MoneyAccountSummaryVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly IJournalPlanner _planner;
        private readonly IJournal _journal;
        private IJournalAccount _account;

        public MoneyAccountSummaryVM(ITrackerConfig config, IJournalPlanner planner, IJournal journal)
        {
            _config = config;
            _planner = planner;
            _journal = journal;
        }

        public Guid AccountId { get { return _account.Id; } }

        public JournalAccountType AccountType { get { return _account.JournalType; } }

        public string AccountDesc { get { return _account.Description; } }

        private decimal _bal;

        public decimal Balance
        {
            // Make sure the Liability accounts display as a Positive # 
            get { return this.AccountType == JournalAccountType.Bank ? _bal : _bal * -1; }
            set
            {
                _bal = value;
                NotifyPropertyChanged(nameof(Balance));
            }
        }


        private ObservableCollection<JournalPlanVM> _listPlans = new ObservableCollection<JournalPlanVM>();
        public ObservableCollection<JournalPlanVM> PlanList { get { return _listPlans; } }


        public bool ShowBudgetData { get { return PlanList.Count > 0; } }

        public decimal BudgetBalance
        {
            get
            {
                decimal bal = _bal;
                if (this.PlanList.Count > 0)
                {

                    foreach (var p in this.PlanList)
                    {
                        switch (p.PlanType)
                        {
                            case JournalPlanType.Receivable:
                                bal += p.Amount;
                                break;
                            case JournalPlanType.Payable:
                                bal -= p.Amount;
                                break;
                            case JournalPlanType.Transfer:
                                if (p.IsParentDebit)
                                {
                                    bal += p.Amount;
                                }
                                else
                                {
                                    bal -= p.Amount;
                                }
                                break;
                        }
                    }
                }
                return this.AccountType == JournalAccountType.Bank ? bal : bal * -1;
            }
        }


        #region Commands
        private RelayCommand _cmdDetail;
        public RelayCommand CmdDetail
        {
            get
            {
                return _cmdDetail ?? (_cmdDetail = new RelayCommand((o) =>
                {
                    TransDetailFilter filter = new TransDetailFilter() { Account = _account };
                    AccountTransactionDetail window = new AccountTransactionDetail(filter);
                    window.Show();
                }));
            }
        }

        private RelayCommand _cmdCreateTrans;
        public RelayCommand CommandCreateTransaction
        {
            get
            {
                return _cmdCreateTrans ?? (_cmdCreateTrans = new RelayCommand((plan) =>
                {
                    if (plan is JournalPlanVM jPlan)
                    {
                        // TODO: Call the UI to create the transaction




                        this.RemoveBudgetPlan(jPlan);
                    }

                }));
            }
        }
        #endregion


        public void Refresh()
        {
            this.Balance = _journal.GetAccountBalance(this.AccountId);
            this.LoadBudgetPlans();
            this.NotifyAll();
        }

        public void LoadAccount(IJournalAccount account)
        {
            _account = account;
            this.Refresh();
        }

        private void LoadBudgetPlans()
        {

            _listPlans.Clear();
            var list = _planner.GetUpcomingPlansForAccount(this.AccountId);
            if (list?.Any() != true) return;

            foreach (var p in list)
            {
                this.AddBudgetPlan(p);
            }
        }

        private void AddBudgetPlan(IJournalPlan plan)
        {
            if (_listPlans.Any(x => x.PlanUID == plan.UID)) return;
            _listPlans.Add(new JournalPlanVM(_account, plan));
            NotifyPropertyChanged(nameof(ShowBudgetData));
        }

        private void RemoveBudgetPlan(JournalPlanVM plan)
        {
            if (!_listPlans.Any(x => x.PlanUID == plan.PlanUID)) return;
            _listPlans.Remove(plan);
            NotifyPropertyChanged(nameof(ShowBudgetData));
        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(ShowBudgetData));
            NotifyPropertyChanged(nameof(Balance));
            NotifyPropertyChanged(nameof(BudgetBalance));
            NotifyPropertyChanged(nameof(AccountDesc));
        }
    }
}
