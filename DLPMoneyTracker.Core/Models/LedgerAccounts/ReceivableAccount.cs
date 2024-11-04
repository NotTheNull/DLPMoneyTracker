
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.LedgerAccounts
{
    public class ReceivableAccount : INominalAccount, ISubLedgerAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType { get { return LedgerType.Receivable; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        public BudgetTrackingType BudgetType { get; set; }
        public decimal DefaultMonthlyBudgetAmount { get; set; } = decimal.Zero;
        public decimal CurrentBudgetAmount { get; set; } = decimal.Zero;
        public IJournalAccount? SummaryAccount { get; set; }


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

            if (cpy is INominalAccount nominal)
            {
                this.BudgetType = nominal.BudgetType;
                this.DefaultMonthlyBudgetAmount = nominal.DefaultMonthlyBudgetAmount;
                this.CurrentBudgetAmount = nominal.CurrentBudgetAmount;
            }
        }

    }
}
