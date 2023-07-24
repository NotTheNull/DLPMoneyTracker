using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class ReceivableAccount : IJournalAccount
    {


        public Guid Id { get; set; }

        public string Description { get; set; }

        public JournalAccountType JournalType { get { return JournalAccountType.Receivable; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        public string MoneyAccountId { get { return string.Empty; } }
        public MoneyAccountType AccountType { get { return MoneyAccountType.NotSet; } }
        public Guid CategoryId { get; set; }

        public decimal MonthlyBudgetAmount { get; set; }

        public bool ExcludeFromBudget { get; set; }



        public ReceivableAccount()
        {
            Id = Guid.NewGuid();
        }
        public ReceivableAccount(IJournalAccount cpy)
        {
            this.Copy(cpy);
        }


        public void Copy(IJournalAccount cpy)
        {
            if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Copy MUST be a Receivable Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            this.CategoryId = cpy.CategoryId;
            this.MonthlyBudgetAmount = cpy.MonthlyBudgetAmount;
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
            this.ExcludeFromBudget = cat.ExcludeFromBudget;
            MonthlyBudgetAmount = cat.DefaultMonthlyBudget;
        }
#pragma warning restore CS0612 // Type or member is obsolete
    }
}
