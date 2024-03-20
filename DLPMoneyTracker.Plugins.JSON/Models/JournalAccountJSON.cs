using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Models
{

    internal sealed class JournalAccountJSON
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType { get; set; }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }
        public DateTime? PreviousBankReconciliationStatementDate { get; set; }


        public decimal MonthlyBudgetAmount { get; set; }
        public bool ExcludeFromBudget { get; set; }

    }
}
