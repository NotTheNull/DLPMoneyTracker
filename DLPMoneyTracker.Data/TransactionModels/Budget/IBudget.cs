using System;

namespace DLPMoneyTracker.Data.TransactionModels.Budget
{
    [Obsolete("Budget Amount is on the Ledger Account")]
    public interface IBudget
    {
        Guid CategoryId { get; }
        int Year { get; }
        int Month { get; }
        decimal BudgetAmount { get; set; }
    }
}