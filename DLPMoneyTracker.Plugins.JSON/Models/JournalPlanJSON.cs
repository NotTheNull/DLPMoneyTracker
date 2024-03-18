using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Models
{
    public enum JournalPlanType
    {
        Payable,
        Receivable,
        Transfer,
        DebtPayment,
        NotSet
    }
    public sealed class JournalPlanJSON 
    {
        public Guid UID { get; set; }

        public Guid DebitAccountId { get; set; }
        
        public Guid CreditAccountId { get; set; }

        public JournalPlanType PlanType { get; set; }

        public string Description { get; set; }

        public string RecurrenceJSON { get; set; }

        public decimal ExpectedAmount { get; set; }



    }
}
