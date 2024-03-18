
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core
{
    public static class StringOverrides
    {
        public static string ToDisplayText(this LedgerType journalType)
        {
            switch (journalType)
            {
                case LedgerType.Payable:
                    return "Accounts Payable";

                case LedgerType.LiabilityLoan:
                    return "Loan";

                case LedgerType.LiabilityCard:
                    return "Credit Card";

                case LedgerType.Receivable:
                    return "Accounts Receivable";

                case LedgerType.Bank:
                    return "Bank";

                default:
                    return "*N/A*";
            }
        }

    }
}
