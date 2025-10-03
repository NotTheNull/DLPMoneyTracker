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

namespace DLPMoneyTracker2.Main.AccountSummary;

public class MoneyAccountSummaryVM : BaseViewModel
{
    private readonly IGetJournalAccountCurrentMonthBalanceUseCase getAccountBalanceUseCase;
    private readonly IGetUpcomingPlansForAccountUseCase getUpcomingPlansUseCase;
    private readonly IFindTransactionForBudgetPlanUseCase findBudgetPlanTransactionUseCase;
    private readonly NotificationSystem notifications;
    private IJournalAccount _account = null!;

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

    public Guid AccountId => _account.Id;

    public LedgerType AccountType => _account.JournalType;

    public string AccountDesc => _account.Description;

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

    private readonly ObservableCollection<JournalPlanVM> _listPlans = [];
    public ObservableCollection<JournalPlanVM> PlanList => _listPlans;

    public bool ShowBudgetData => PlanList.Count > 0;

    public decimal BudgetBalance
    {
        get
        {
            decimal bal = _bal;
            if (this.PlanList.Count > 0)
            {
                foreach (var p in this.PlanList)
                {
                    bal += p.PlanType switch
                    {
                        BudgetPlanType.Receivable => p.Amount,
                        BudgetPlanType.Payable => p.Amount * -1,
                        BudgetPlanType.Transfer => p.Amount * (p.IsParentDebit ? 1 : -1),
                        BudgetPlanType.DebtPayment => p.Amount * ((this.AccountType == LedgerType.Bank) ? -1 : 1),
                        _ => 0
                    };
                }
            }
            return this.AccountType == LedgerType.Bank ? bal : bal * -1;
        }
    }

    #region Commands

    public RelayCommand CommandDetails =>
        new((o) =>
        {
            TransDetailFilter filter = new() { Account = _account };
            AccountTransactionDetail window = new(filter);
            window.Show();
        });

    public RelayCommand CommandCreateTransaction =>
        new((plan) =>
        {
            if (plan is JournalPlanVM jPlan)
            {
                IJournalEntryVM transVM = jPlan.PlanType switch
                {
                    BudgetPlanType.Payable => UICore.DependencyHost.GetRequiredService<ExpenseJournalEntryVM>(),
                    BudgetPlanType.Receivable => UICore.DependencyHost.GetRequiredService<IncomeJournalEntryVM>(),
                    BudgetPlanType.Transfer => UICore.DependencyHost.GetRequiredService<TransferJournalEntryVM>(),
                    BudgetPlanType.DebtPayment => UICore.DependencyHost.GetRequiredService<DebtPaymentJournalEntryVM>(),
                    _ => throw new InvalidOperationException($"No transaction definition for plan type {jPlan.PlanType} ")
                };

                transVM.FillFromPlan(jPlan.ThePlan);
                RecordJournalEntry window = new(transVM);
                window.Show();

                this.RemoveBudgetPlan(jPlan);
            }
        });

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