using System;

namespace DLPMoneyTracker.Data.TransactionModels.Budget
{
    public interface IBudget
    {
        Guid CategoryId { get; }
        int Year { get; }
        int Month { get; }
        decimal BudgetAmount { get; set; }
    }
}