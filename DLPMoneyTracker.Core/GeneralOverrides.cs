
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core
{
    public static class GeneralOverrides
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


        public static int ToLedgerNumber(this LedgerType journalType)
        {
            switch(journalType)
            {
                case LedgerType.Bank:
                    return 1;
                case LedgerType.LiabilityCard:
                    return 2;
                case LedgerType.LiabilityLoan:
                    return 3;
                case LedgerType.Receivable:
                    return 4;
                case LedgerType.Payable:
                    return 5;
                case LedgerType.NotSet:
                    return 6;
                default:
                    return 0;
            }
        }
    }
}
