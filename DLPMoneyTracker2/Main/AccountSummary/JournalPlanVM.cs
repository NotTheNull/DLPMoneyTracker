using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;

namespace DLPMoneyTracker2.Main.AccountSummary
{
    public class JournalPlanVM(IJournalAccount act, IBudgetPlan plan) : BaseViewModel
    {
        private readonly IBudgetPlan _plan = plan;
        private readonly IJournalAccount _actParent = act;

        private Guid ParentAccountId => _actParent.Id;
        public IBudgetPlan ThePlan => _plan;
        public Guid PlanUID => _plan.UID;
        public bool IsParentDebit => _plan.DebitAccountId == this.ParentAccountId;
        public string Description => _plan.Description;
        public BudgetPlanType PlanType => _plan.PlanType;
        public string PlanTypeDescription => _plan.PlanType.ToString();
        public decimal Amount => _plan.ExpectedAmount;
        public DateTime NextDueDate => _plan.NextOccurrence;
    }
}