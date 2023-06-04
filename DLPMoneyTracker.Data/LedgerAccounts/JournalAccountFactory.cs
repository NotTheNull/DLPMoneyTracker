using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    internal class JournalAccountFactory
    {
        public static IJournalAccount Build(JournalAccountJSON data)
        {
            switch (data.JournalType)
            {
                case JounalAccountType.Bank:
                    return new BankAccount(data);
                case JounalAccountType.LiabilityCard:
                    return new CreditCardAccount(data);
                case JounalAccountType.LiabilityLoan:
                    return new LoanAccount(data);
                case JounalAccountType.Payable:
                    return new PayableAccount(data);
                case JounalAccountType.Receivable:
                    return new ReceivableAccount(data);
                default:
                    throw new NotSupportedException(string.Format("Ledger Type [{0}] is not supported", data.JournalType.ToString()));
            }
        }


    }
}
