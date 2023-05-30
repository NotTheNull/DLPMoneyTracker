using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class LoanAccount : ILedgerAccount, IMoneyAccountReference
    {
        public Guid Id { get; private set; }

        public string Description { get; set; }

        public LedgerTypes LedgerType { get { return LedgerTypes.LiabilityLoan; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }


        public string MoneyAccountId { get; set; }

        public MoneyAccountType AccountType { get { return MoneyAccountType.Loan; } }
    }
}
