using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace MoneyTrackerWebApp.Models.Summary
{
    public class SummaryItemPlanVM
    {
        private readonly IJournalAccount account;
        private readonly IBudgetPlan plan;

        public SummaryItemPlanVM(IJournalAccount act, IBudgetPlan plan)
        {
            this.account = act;
            this.plan = plan;
        }


        private Guid ParentAccountId { get { return account.Id; } }

        public IBudgetPlan ThePlan { get { return plan; } }

        public Guid PlanUID { get { return plan.UID; } }

        public bool IsParentDebit { get { return plan.DebitAccountId == this.ParentAccountId; } }

        public string Description { get { return plan.Description; } }

        public BudgetPlanType PlanType { get { return plan.PlanType; } }

        public string PlanTypeDescription { get { return plan.PlanType.ToString(); } }

        public decimal Amount { get { return plan.ExpectedAmount; } }

        public DateTime NextDueDate { get { return plan.NextOccurrence; } }

    }
}
