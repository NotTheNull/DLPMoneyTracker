using System;

namespace DLPMoneyTracker.Data.TransactionModels.Budget
{
    public class BudgetJSON : IBudget
    {
        public Guid CategoryId { get; set; }

        public decimal BudgetAmount { get; set; }
    }
}