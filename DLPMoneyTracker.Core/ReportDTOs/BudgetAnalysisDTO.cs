using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.ReportDTOs
{
    /// <summary>
    /// This report is going to compare the Receivables to the Payables and determine Net Gain/Loss
    /// </summary>
    public class BudgetAnalysisDTO
    {
        public string AccountName { get; set; }
        public string IncomeOrExpense { get; set; } // Will either be "Income" or "Expense"
        public DateTime TransactionDate { get; set; }
        public string TransactionDescription { get; set; }
        public decimal TransactionAmount { get; set; }

    }
}
