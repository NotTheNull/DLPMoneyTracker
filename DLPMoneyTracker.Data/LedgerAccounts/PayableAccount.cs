using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class PayableAccount : ILedgerAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerTypes LedgerType { get { return LedgerTypes.Payable; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        public bool ShouldAffectBudget { get; set; }
        public decimal DefaultMonthlyBudget { get; set; }

        public string MoneyAccountId { get { return string.Empty; } }
        public MoneyAccountType AccountType { get { return MoneyAccountType.NotSet; } }
        public Guid CategoryId { get; set; }

        public PayableAccount()
        {
            Id = Guid.NewGuid();
        }
        public PayableAccount(ILedgerAccount cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(ILedgerAccount cpy)
        {
            if (cpy.LedgerType != this.LedgerType) throw new InvalidOperationException("Copy MUST be a Payable Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            this.CategoryId = cpy.CategoryId;
        }


#pragma warning disable CS0612 // Type or member is obsolete
        public PayableAccount(TransactionCategory old) : this()
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
