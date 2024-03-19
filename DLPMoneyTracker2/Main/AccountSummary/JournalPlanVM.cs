

using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;

namespace DLPMoneyTracker2.Main.AccountSummary
{
    public class JournalPlanVM : BaseViewModel
    {
        private readonly IBudgetPlan _plan;
        private readonly IJournalAccount _actParent;

        public JournalPlanVM(IJournalAccount act, IBudgetPlan plan)
        {
            _plan = plan;
            _actParent = act;
        }

        private Guid ParentAccountId { get { return _actParent.Id; } }

        public IBudgetPlan ThePlan { get { return _plan; } }

        public Guid PlanUID { get { return _plan.UID; } }

        public bool IsParentDebit { get { return _plan.DebitAccountId == this.ParentAccountId; } }

        public string Description { get { return _plan.Description; } }

        public BudgetPlanType PlanType { get { return _plan.PlanType; } }

        public string PlanTypeDescription { get { return _plan.PlanType.ToString(); } }

        public decimal Amount { get { return _plan.ExpectedAmount; } }

        public DateTime NextDueDate { get { return _plan.NextOccurrence; } }
    }
}