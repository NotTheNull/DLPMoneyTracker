using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Models
{

    internal sealed class JournalAccountJSON
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType { get; set; }
        public BudgetTrackingType BudgetType { get; set; } = BudgetTrackingType.DO_NOT_TRACK;

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }
        public DateTime? PreviousBankReconciliationStatementDate { get; set; }


    }
}
