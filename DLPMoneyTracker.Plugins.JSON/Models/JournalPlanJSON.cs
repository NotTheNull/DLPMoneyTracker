using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Plugins.JSON.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Models
{
    
    internal sealed class JournalPlanJSON 
    {
        public Guid UID { get; set; }

        public BudgetPlanType PlanType { get; set; }

        public string Description { get; set; }

        public string RecurrenceJSON { get; set; }

        public Guid DebitAccountId { get; set; }
        
        public Guid CreditAccountId { get; set; }

        public decimal ExpectedAmount { get; set; }

        public void Copy(IBudgetPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);
            
            this.UID = plan.UID;
            this.PlanType = plan.PlanType;
            this.Description = plan.Description;
            this.DebitAccountId = plan.DebitAccountId;
            this.CreditAccountId = plan.CreditAccountId;
            this.ExpectedAmount = plan.ExpectedAmount;

            JSONScheduleRecurrenceAdapter adapter = new JSONScheduleRecurrenceAdapter();
            adapter.Copy(plan.Recurrence);
            this.RecurrenceJSON = adapter.ExportJSON();
        }

    }
}
