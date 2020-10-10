using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.Data.TransactionModels.Budget
{
    public interface IBudget
    {
        Guid CategoryId { get; }
        decimal BudgetAmount { get; set; }
    }
}
