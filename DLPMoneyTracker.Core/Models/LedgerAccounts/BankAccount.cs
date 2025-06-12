
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.LedgerAccounts
{
    public class BankAccount : IMoneyAccount
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Description { get; set; } = string.Empty;

        public LedgerType JournalType { get { return LedgerType.Bank; } }
        public int OrderBy { get; set; } = 0;
        public DateTime? DateClosedUTC { get; set; }

        public ICSVMapping Mapping { get; } = new();

        public BankAccount() {}
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

            if(cpy is IMoneyAccount money)
            {
                this.Mapping.Copy(money.Mapping);
            }
        }

    }
}
