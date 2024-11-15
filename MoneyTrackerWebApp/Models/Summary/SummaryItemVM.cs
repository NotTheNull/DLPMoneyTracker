using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace MoneyTrackerWebApp.Models.Summary
{
    public class SummaryItemVM
    {
        private readonly IGetJournalAccountCurrentMonthBalanceUseCase getAccountBalanceUseCase;
        private readonly IGetUpcomingPlansForAccountUseCase getUpcomingPlansUseCase;
        private readonly IFindTransactionForBudgetPlanUseCase findBudgetPlanTransactionUseCase;

        public SummaryItemVM(
            IGetJournalAccountCurrentMonthBalanceUseCase getAccountBalanceUseCase,
            IGetUpcomingPlansForAccountUseCase getUpcomingPlansUseCase,
            IFindTransactionForBudgetPlanUseCase findBudgetPlanTransactionUseCase)
        {
            this.getAccountBalanceUseCase = getAccountBalanceUseCase;
            this.getUpcomingPlansUseCase = getUpcomingPlansUseCase;
            this.findBudgetPlanTransactionUseCase = findBudgetPlanTransactionUseCase;
        }


        private IJournalAccount _account;

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
            }
        }

        public List<SummaryItemPlanVM> PlanList { get; } = new List<SummaryItemPlanVM>();

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



        public void LoadAccount(IJournalAccount act)
        {
            _account = act;
            this.Refresh();
        }

        public void Refresh()
        {
            this.Balance = getAccountBalanceUseCase.Execute(this.AccountId);
            this.LoadBudgetPlans();
        }

        private void LoadBudgetPlans()
        {
            this.PlanList.Clear();
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
            if (this.PlanList.Any(x => x.PlanUID == plan.UID)) return;
            this.PlanList.Add(new SummaryItemPlanVM(this._account, plan));
        }

        public void RemoveBudgetPlan(SummaryItemPlanVM plan)
        {
            if (!this.PlanList.Any(x => x.PlanUID == plan.PlanUID)) return;
            this.PlanList.Remove(plan);
        }
    }

}
