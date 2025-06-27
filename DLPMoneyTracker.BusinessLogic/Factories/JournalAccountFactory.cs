using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.Factories
{
    public static class JournalAccountFactory
    {
        public static IJournalAccount Build(IJournalAccount acct)
        {
            if (acct.Id == SpecialAccount.DebtInterest.Id) return SpecialAccount.DebtInterest;
            if (acct.Id == SpecialAccount.DebtReduction.Id) return SpecialAccount.DebtReduction;
            if (acct.Id == SpecialAccount.InitialBalance.Id) return SpecialAccount.InitialBalance;
            if (acct.Id == SpecialAccount.UnlistedAdjustment.Id) return SpecialAccount.UnlistedAdjustment;
            if (acct.Id == SpecialAccount.InvalidAccount.Id) return SpecialAccount.InvalidAccount;

            return acct.JournalType switch
            {
                LedgerType.Bank => new BankAccount(acct),
                LedgerType.LiabilityCard => new CreditCardAccount(acct),
                LedgerType.LiabilityLoan => new LoanAccount(acct),
                LedgerType.Payable => new PayableAccount(acct),
                LedgerType.Receivable => new ReceivableAccount(acct),
                _ => throw new NotSupportedException(string.Format("Ledger Type [{0}] is not supported", acct.JournalType.ToString()))
            };
        }

        public static IJournalAccount Build(LedgerType jType, string description, int orderBy = 99)
        {
            return jType switch
            {
                LedgerType.Bank => new BankAccount() { Description = description, OrderBy = orderBy },
                LedgerType.LiabilityCard => new CreditCardAccount() { Description = description, OrderBy = orderBy },
                LedgerType.LiabilityLoan => new LoanAccount() { Description = description, OrderBy = orderBy },
                LedgerType.Payable => new PayableAccount() { Description = description },
                LedgerType.Receivable => new ReceivableAccount() { Description = description },
                _ => throw new NotSupportedException(string.Format("Ledger Type [{0}] is not supported", jType.ToString()))
            };
        }
    }
}