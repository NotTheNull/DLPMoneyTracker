using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class ReceivableAccount : ILedgerAccount, ITransactionCategoryReference
    {
        public Guid Id { get; private set; }

        public string Description { get; set; }

        public LedgerTypes LedgerType { get { return LedgerTypes.Receivable; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        public bool ShouldAffectBudget { get; set; }
        public decimal DefaultMonthlyBudget { get; set; }

        public Guid CategoryId { get; set; }

        public ReceivableAccount()
        {
            Id = Guid.NewGuid();
        }

#pragma warning disable CS0612 // Type or member is obsolete
        public ReceivableAccount(TransactionCategory old) : this()
        {
            this.Convert(old);
        }


        public void Convert(TransactionCategory cat)
        {
            CategoryId = cat.ID;
            Description = cat.Name;
            DateClosedUTC = cat.DateDeletedUTC;
            ShouldAffectBudget = !cat.ExcludeFromBudget;
            DefaultMonthlyBudget = cat.DefaultMonthlyBudget;
        }
#pragma warning restore CS0612 // Type or member is obsolete
    }
}
