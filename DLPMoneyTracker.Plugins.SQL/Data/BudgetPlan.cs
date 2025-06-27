using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DLPMoneyTracker.Plugins.SQL.Data
{
    public class BudgetPlan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public Guid PlanUID { get; set; } = Guid.NewGuid();
        public BudgetPlanType PlanType { get; set; } = BudgetPlanType.NotSet;

        [Required, StringLength(100)]
        public string Description { get; set; } = string.Empty;

        public RecurrenceFrequency Frequency { get; set; } = RecurrenceFrequency.Annual;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public decimal ExpectedAmount { get; set; } = decimal.Zero;

        public int DebitId { get; set; }
        public virtual Account? Debit { get; set; }

        public int CreditId { get; set; }
        public virtual Account? Credit { get; set; }
    }
}