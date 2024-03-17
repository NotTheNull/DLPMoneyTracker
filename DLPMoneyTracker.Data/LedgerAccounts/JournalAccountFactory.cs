using System;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class JournalAccountFactory
    {
        public static IJournalAccount Build(JournalAccountJSON data)
        {
            switch (data.JournalType)
            {
                case LedgerType.Bank:
                    return new BankAccount(data);

                case LedgerType.LiabilityCard:
                    return new CreditCardAccount(data);

                case LedgerType.LiabilityLoan:
                    return new LoanAccount(data);

                case LedgerType.Payable:
                    return new PayableAccount(data);

                case LedgerType.Receivable:
                    return new ReceivableAccount(data);

                default:
                    throw new NotSupportedException(string.Format("Ledger Type [{0}] is not supported", data.JournalType.ToString()));
            }
        }

        public static IJournalAccount Build(string desc, LedgerType jtype, decimal budget = decimal.Zero, int orderBy = 99)
        {
            switch (jtype)
            {
                case LedgerType.Bank:
                    return new BankAccount() { Description = desc, OrderBy = orderBy };

                case LedgerType.LiabilityCard:
                    return new CreditCardAccount() { Description = desc, OrderBy = orderBy };

                case LedgerType.LiabilityLoan:
                    return new LoanAccount() { Description = desc, OrderBy = orderBy };

                case LedgerType.Payable:
                    return new PayableAccount() { Description = desc, MonthlyBudgetAmount = budget };

                case LedgerType.Receivable:
                    return new ReceivableAccount() { Description = desc, MonthlyBudgetAmount = budget };

                default:
                    throw new NotSupportedException(string.Format("Ledger Type [{0}] is not supported", jtype.ToString()));
            }
        }

        public static void Update(ref IJournalAccount account, string desc, decimal budget = decimal.Zero, int orderBy = 99)
        {
            if (account is BankAccount bank)
            {
                bank.Description = desc;
                bank.OrderBy = orderBy;
            }
            else if (account is CreditCardAccount creditCard)
            {
                creditCard.Description = desc;
                creditCard.OrderBy = orderBy;
            }
            else if (account is LoanAccount loanAccount)
            {
                loanAccount.Description = desc;
                loanAccount.OrderBy = orderBy;
            }
            else if (account is PayableAccount payableAccount)
            {
                payableAccount.Description = desc;
                payableAccount.MonthlyBudgetAmount = budget;
            }
            else if (account is ReceivableAccount receivableAccount)
            {
                receivableAccount.Description = desc;
                receivableAccount.MonthlyBudgetAmount = budget;
            }
        }
    }
}