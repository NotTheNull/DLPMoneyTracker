namespace DLPMoneyTracker.Core.ReportDTOs
{
    /// <summary>
    /// This report is going to compare the Receivables to the Payables and determine Net Gain/Loss
    /// </summary>
    public class BudgetAnalysisDTO
    {
        public string AccountName { get; set; } = string.Empty;
        public string IncomeOrExpense { get; set; } = string.Empty; // Will either be "Income" or "Expense"
        public DateTime TransactionDate { get; set; }
        public string TransactionDescription { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; }
    }
}