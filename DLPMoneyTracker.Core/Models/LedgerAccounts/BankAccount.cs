
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.LedgerAccounts
{
    public class BankAccount : IMoneyAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType { get { return LedgerType.Bank; } }
        public int OrderBy { get; set; }
        public DateTime? DateClosedUTC { get; set; }

        public string LedgerNumber { get { return string.Format("{0}-{1}-{2}", JournalType.ToLedgerNumber(), CategoryId, SubLedgerId); } }

        public int CategoryId { get; set; } = -1;

        public int SubLedgerId { get; set; } = -1;

        public BankAccount()
        {
            Id = Guid.NewGuid();
        }

        public BankAccount(IJournalAccount cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(IJournalAccount cpy)
        {
            if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Copy MUST be a Bank Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            this.CategoryId = cpy.CategoryId;
            this.SubLedgerId = cpy.SubLedgerId;
        }

    }
}
