
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.LedgerAccounts
{
    public class ReceivableAccount : INominalAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType        { get { return LedgerType.Receivable; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        public string LedgerNumber { get { return string.Format("{0}-{1}-{2}", JournalType.ToLedgerNumber(), CategoryId, SubLedgerId); } }

        public int CategoryId { get; set; }

        public int SubLedgerId { get; set; }



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
            this.SubLedgerId = cpy.SubLedgerId;

        }

    }
}
