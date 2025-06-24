using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.Factories
{
    public class JournalAccountFactory
    {
        
        public JournalAccountFactory()
        {
            
        }


        public IJournalAccount Build(IJournalAccount acct)
        {
            if (acct.Id == SpecialAccount.DebtInterest.Id) return SpecialAccount.DebtInterest;
            if (acct.Id == SpecialAccount.DebtReduction.Id) return SpecialAccount.DebtReduction;
            if (acct.Id == SpecialAccount.InitialBalance.Id) return SpecialAccount.InitialBalance;
            if (acct.Id == SpecialAccount.UnlistedAdjusment.Id) return SpecialAccount.UnlistedAdjusment;

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

        public IJournalAccount Build(LedgerType jType, string description, int orderBy = 99)
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
