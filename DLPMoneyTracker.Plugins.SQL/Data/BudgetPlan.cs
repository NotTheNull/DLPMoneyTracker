using DLPMoneyTracker.Core.Models.BudgetPlan;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Data
{
    
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
