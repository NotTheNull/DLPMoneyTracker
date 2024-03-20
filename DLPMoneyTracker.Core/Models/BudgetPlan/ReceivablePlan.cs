using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.BudgetPlan
{
    public class ReceivablePlan : IBudgetPlan
    {
        public Guid UID { get; set; } = Guid.NewGuid();

        public BudgetPlanType PlanType { get { return BudgetPlanType.Receivable; } }

        public string Description { get; set; }

        public decimal ExpectedAmount { get; set; }

        private List<LedgerType> _validDebits = new List<LedgerType>()
        {
            LedgerType.Bank
        };


        public List<LedgerType> ValidDebitAccountTypes { get { return _validDebits; } }

        public IJournalAccount DebitAccount { get; set; }

        public Guid DebitAccountId { get { return DebitAccount?.Id ?? Guid.Empty; } }

        public string DebitAccountName { get { return DebitAccount?.Description ?? string.Empty; } }



        private List<LedgerType> _validCredits = new List<LedgerType>()
        {
            LedgerType.Receivable
        };


        public List<LedgerType> ValidCreditAccountTypes { get { return _validCredits; } }

        public IJournalAccount CreditAccount { get; set; }

        public Guid CreditAccountId { get { return CreditAccount?.Id ?? Guid.Empty; } }

        public string CreditAccountName { get { return CreditAccount?.Description ?? string.Empty; } }


        public IScheduleRecurrence Recurrence { get; set; }
        public DateTime NotificationDate { get { return Recurrence.NotificationDate; } }
        public DateTime NextOccurrence { get { return Recurrence.NextOccurrence; } }



        public bool IsValid()
        {
            if (this.DebitAccount is null || this.CreditAccount is null) return false;
            if (!this.ValidDebitAccountTypes.Contains(DebitAccount.JournalType)) return false;
            if (!this.ValidCreditAccountTypes.Contains(CreditAccount.JournalType)) return false;
            if (string.IsNullOrWhiteSpace(this.Description)) return false;
            if (ExpectedAmount <= decimal.Zero) return false;

            return true;
        }
        public void Copy(IBudgetPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);
            if (plan.PlanType != this.PlanType) throw new InvalidOperationException("Plan types do not match");

            this.UID = plan.UID;
            this.CreditAccount = plan.CreditAccount;
            this.DebitAccount = plan.DebitAccount;
            this.Description = plan.Description;
            this.Recurrence = plan.Recurrence;
            this.ExpectedAmount = plan.ExpectedAmount;

        }
    }
}
