using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.BankReconciliation
{
    public class BankReconciliationDTO
    {
        public IJournalAccount BankAccount { get; set; }
        public DateRange StatementDate { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }

        public void Copy(BankReconciliationDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            this.StatementDate = dto.StatementDate;
            this.StartingBalance = dto.StartingBalance;
            this.EndingBalance = dto.EndingBalance;
        }
    }
}
