using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class JournalAccountFactory
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

        public static IJournalAccount Build(string desc, JournalAccountType jtype, decimal budget = decimal.Zero)
        {
            switch (jtype)
            {
                case JournalAccountType.Bank:
                    return new BankAccount() { Description = desc, };
                case JournalAccountType.LiabilityCard:
                    return new CreditCardAccount() { Description = desc, };
                case JournalAccountType.LiabilityLoan:
                    return new LoanAccount() { Description = desc, };
                case JournalAccountType.Payable:
                    return new PayableAccount() { Description = desc, MonthlyBudgetAmount = budget };
                case JournalAccountType.Receivable:
                    return new ReceivableAccount() { Description = desc, MonthlyBudgetAmount = budget };
                default:
                    throw new NotSupportedException(string.Format("Ledger Type [{0}] is not supported", jtype.ToString()));
            }
        }
    }
}
