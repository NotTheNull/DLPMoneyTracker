using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.Plugins.JSON.Models
{
    public sealed class JournalAccountJSON
    {
        public Guid Id { get; set; }

        public string Description { get; set; } = string.Empty;

        public LedgerType JournalType { get; set; }
        public BudgetTrackingType BudgetType { get; set; } = BudgetTrackingType.DO_NOT_TRACK;
        public decimal DefaultMonthlyBudgetAmount { get; set; } = decimal.Zero;
        public decimal CurrentBudgetAmount { get; set; } = decimal.Zero;

        public int OrderBy { get; set; } = 1;

        public DateTime? DateClosedUTC { get; set; }
        public DateTime? PreviousBankReconciliationStatementDate { get; set; }
        public Guid? SummaryAccountUID { get; set; }

        public CSVMapping? Mapping { get; set; }
    }
}