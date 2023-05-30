using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public interface ITransactionCategoryReference
    {
        Guid Id { get; }
        string Description { get; set; }
        bool ShouldAffectBudget { get; set; }
        decimal DefaultMonthlyBudget { get; set; }
        DateTime? DateClosedUTC { get; set; }
        Guid CategoryId {get; } // Reference to legacy TransactionCategory

#pragma warning disable CS0612 // Type or member is obsolete
        void Convert(TransactionCategory cat);
#pragma warning restore CS0612 // Type or member is obsolete
    }
}
