using System;

namespace DLPMoneyTracker.Data.TransactionModels.Budget
{
    public interface IBudget
    {
        Guid CategoryId { get; }
        decimal BudgetAmount { get; set; }
    }
}