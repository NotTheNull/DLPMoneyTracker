using DLPMoneyTracker.Core.Models.BankReconciliation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Models
{
    internal sealed class BankReconciliationJSON
    {
        public Guid BankAccountId { get; set; }

        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }

        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }

        // This is only here for compatibility; no reason to keep writing it back
        public List<JournalEntryJSON> TransactionList { get; set; }

        internal void Copy(BankReconciliationDTO rec)
        {
            ArgumentNullException.ThrowIfNull(rec);

            this.BankAccountId = rec.BankAccount.Id;
            this.StartingDate = rec.StatementDate.Begin;
            this.EndingDate = rec.StatementDate.End;
            this.StartingBalance = rec.StartingBalance;
            this.EndingBalance = rec.EndingBalance;
        }
    }
}
