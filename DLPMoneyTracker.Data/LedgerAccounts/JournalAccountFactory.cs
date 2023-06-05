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
                case JournalAccountType.Bank:
                    return new BankAccount(data);
                case JournalAccountType.LiabilityCard:
                    return new CreditCardAccount(data);
                case JournalAccountType.LiabilityLoan:
                    return new LoanAccount(data);
                case JournalAccountType.Payable:
                    return new PayableAccount(data);
                case JournalAccountType.Receivable:
                    return new ReceivableAccount(data);
                default:
                    throw new NotSupportedException(string.Format("Ledger Type [{0}] is not supported", data.JournalType.ToString()));
            }
        }


    }
}
