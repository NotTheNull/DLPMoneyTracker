using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker2.LedgerEntry;
using DLPMoneyTracker2.Main.TransactionList;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.AccountSummary
{
    public class MoneyAccountSummaryVM : BaseViewModel
    {
        private readonly IGetJournalAccountCurrentMonthBalanceUseCase getAccountBalanceUseCase;
        private readonly IGetUpcomingPlansForAccountUseCase getUpcomingPlansUseCase;
        private readonly IFindTransactionForBudgetPlanUseCase findBudgetPlanTransactionUseCase;
        private readonly NotificationSystem notifications;
        private IJournalAccount _account;

        // TODO: Loans aren't displaying
        public MoneyAccountSummaryVM(
            IGetJournalAccountCurrentMonthBalanceUseCase getAccountBalanceUseCase,
            IGetUpcomingPlansForAccountUseCase getUpcomingPlansUseCase,
            IFindTransactionForBudgetPlanUseCase findBudgetPlanTransactionUseCase,
            NotificationSystem notifications)
        {
            this.getAccountBalanceUseCase = getAccountBalanceUseCase;
            this.getUpcomingPlansUseCase = getUpcomingPlansUseCase;
            this.findBudgetPlanTransactionUseCase = findBudgetPlanTransactionUseCase;
            this.notifications = notifications;
            this.notifications.TransactionsModified += Notifications_TransactionsModified;
        }

        private void Notifications_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
            if (this.AccountId != debitAccountId && this.AccountId != creditAccountId) return;

            this.Refresh();
        }


        public Guid AccountId { get { return _account.Id; } }

        public LedgerType AccountType { get { return _account.JournalType; } }

        public string AccountDesc { get { return _account.Description; } }

        private decimal _bal;

        public decimal Balance
        {
            // Make sure the Liability accounts display as a Positive #
            get { return this.AccountType == LedgerType.Bank ? _bal : _bal * -1; }
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
                            case BudgetPlanType.Receivable:
                                bal += p.Amount;
                                break;

                            case BudgetPlanType.Payable:
                                bal -= p.Amount;
                                break;

                            case BudgetPlanType.Transfer:
                                if (p.IsParentDebit)
                                {
                                    bal += p.Amount;
                                }
                                else
                                {
                                    bal -= p.Amount;
                                }
                                break;

                            case BudgetPlanType.DebtPayment:
                                if (this.AccountType == LedgerType.Bank)
                                {
                                    bal -= p.Amount;
                                }
                                else
                                {
                                    bal += p.Amount;
                                }
                                break;
                        }
                    }
                }
                return this.AccountType == LedgerType.Bank ? bal : bal * -1;
            }
        }

        #region Commands

        private RelayCommand _cmdDetail;

        public RelayCommand CommandDetails
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
                        IJournalEntryVM transVM;
                        switch (jPlan.PlanType)
                        {
                            case BudgetPlanType.Payable:
                                transVM = UICore.DependencyHost.GetRequiredService<ExpenseJournalEntryVM>();
                                break;

                            case BudgetPlanType.Receivable:
                                transVM = UICore.DependencyHost.GetRequiredService<IncomeJournalEntryVM>();
                                break;

                            case BudgetPlanType.Transfer:
                                transVM = UICore.DependencyHost.GetRequiredService<TransferJournalEntryVM>();
                                break;

                            case BudgetPlanType.DebtPayment:
                                transVM = UICore.DependencyHost.GetRequiredService<DebtPaymentJournalEntryVM>();
                                break;

                            default:
                                return;
                        }
                        transVM.FillFromPlan(jPlan.ThePlan);
                        RecordJournalEntry window = new RecordJournalEntry(transVM);
                        window.Show();

                        this.RemoveBudgetPlan(jPlan);
                    }
                }));
            }
        }

        #endregion Commands

        public void Refresh()
        {
            this.Balance = getAccountBalanceUseCase.Execute(this.AccountId);
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
            var list = getUpcomingPlansUseCase.Execute(this.AccountId);
            if (list?.Any() != true) return;

            foreach (var p in list)
            {
                // See if we already have a transaction for this budget plan
                var record = findBudgetPlanTransactionUseCase.Execute(p, _account);
                if (record != null) continue;

                this.AddBudgetPlan(p);
            }
        }

        private void AddBudgetPlan(IBudgetPlan plan)
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