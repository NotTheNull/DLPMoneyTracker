using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.BankReconciliation
{
    public class BankReconciliationOverviewDTO
    {
        public IJournalAccount BankAccount { get; set; }
        public List<BankReconciliationDTO> ReconciliationList { get; set; }

    }
}
