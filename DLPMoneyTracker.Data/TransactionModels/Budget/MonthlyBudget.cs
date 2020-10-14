using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels.Budget
{
    public class MonthlyBudget : IBudget
    {
        [JsonIgnore]
        public TransactionCategory Category { get; set; }

        public Guid CategoryId { get { return this.Category?.ID ?? Guid.Empty; } }

        public decimal BudgetAmount { get; set; }
    }
}