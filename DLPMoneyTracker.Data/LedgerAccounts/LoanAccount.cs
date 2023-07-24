using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class LoanAccount : IJournalAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public JournalAccountType JournalType { get { return JournalAccountType.LiabilityLoan; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }


        public string MoneyAccountId { get; set; }

        public MoneyAccountType AccountType { get { return MoneyAccountType.Loan; } }

        public Guid CategoryId { get { return Guid.Empty; } }

        public decimal MonthlyBudgetAmount { get { return decimal.Zero; } }

        public bool ExcludeFromBudget { get { return false; } }


        public LoanAccount()
        {
            this.Id = Guid.NewGuid();
        }
        public LoanAccount(IJournalAccount cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(IJournalAccount cpy)
        {
            if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Copy MUST be a Loan Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            this.MoneyAccountId = cpy.MoneyAccountId;
            
        }

#pragma warning disable CS0612 // Type or member is obsolete
        public LoanAccount(MoneyAccount old) : this()

        {
            this.Convert(old);
        }

        public void Convert(MoneyAccount act)
        {
            MoneyAccountId = act.ID;
            Description = act.Description;
            OrderBy = act.OrderBy;
            DateClosedUTC = act.DateClosedUTC;
        }
#pragma warning restore CS0612 // Type or member is obsolete
    }
}
