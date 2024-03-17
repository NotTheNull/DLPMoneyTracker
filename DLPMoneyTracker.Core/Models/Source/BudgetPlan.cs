using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.Source
{
    public enum RecurrenceFrequency
    {
        BiWeekly, // every two weeks [26 times a year]
        SemiMonthly, // twice a month [24 times a year]
        Monthly,
        SemiAnnual,
        Annual
    }

    public enum BudgetPlanType
    {
        Payable,
        Receivable,
        Transfer,
        DebtPayment,
        NotSet
    }

    public class BudgetPlan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid PlanUID { get; set; }
        public BudgetPlanType PlanType { get; set; }
        [Required, StringLength(100)]
        public string Description { get; set; } = string.Empty;
        public RecurrenceFrequency Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public Account Debit { get; set; }
        public Account Credit { get; set; }

    }
}
