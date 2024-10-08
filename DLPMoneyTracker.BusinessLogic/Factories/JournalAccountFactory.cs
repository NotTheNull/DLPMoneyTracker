﻿using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            switch (acct.JournalType)
            {
                case LedgerType.Bank:
                    return new BankAccount(acct);
                case LedgerType.LiabilityCard:
                    return new CreditCardAccount(acct);
                case LedgerType.LiabilityLoan:
                    return new LoanAccount(acct);
                case LedgerType.Payable:
                    return new PayableAccount(acct);
                case LedgerType.Receivable:
                    return new ReceivableAccount(acct);
                default:
                    // Special accounts should be stored in the DB but don't need to be imported
                    if (acct.Id == SpecialAccount.DebtInterest.Id) return SpecialAccount.DebtInterest;
                    if (acct.Id == SpecialAccount.DebtReduction.Id) return SpecialAccount.DebtReduction;
                    if (acct.Id == SpecialAccount.InitialBalance.Id) return SpecialAccount.InitialBalance;
                    if (acct.Id == SpecialAccount.UnlistedAdjusment.Id) return SpecialAccount.UnlistedAdjusment;

                    throw new NotSupportedException(string.Format("Ledger Type [{0}] is not supported", acct.JournalType.ToString()));
            }
        }

        public IJournalAccount Build(LedgerType jType, string desc, int orderBy = 99)
        {
            switch (jType)
            {
                case LedgerType.Bank:
                    return new BankAccount() { Description = desc, OrderBy = orderBy };

                case LedgerType.LiabilityCard:
                    return new CreditCardAccount() { Description = desc, OrderBy = orderBy };

                case LedgerType.LiabilityLoan:
                    return new LoanAccount() { Description = desc, OrderBy = orderBy };

                case LedgerType.Payable:
                    return new PayableAccount() { Description = desc };

                case LedgerType.Receivable:
                    return new ReceivableAccount() { Description = desc };

                default:
                    throw new NotSupportedException(string.Format("Ledger Type [{0}] is not supported", jType.ToString()));
            }
        }






    }
}
