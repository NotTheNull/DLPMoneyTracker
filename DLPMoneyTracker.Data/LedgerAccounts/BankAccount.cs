using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class BankAccount : IJournalAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public JournalAccountType JournalType { get { return JournalAccountType.Bank; } }
        public int OrderBy { get; set; }
        public DateTime? DateClosedUTC { get; set; }

        // For backwards compatibility
        
        //public string MoneyAccountId { get; set; }
        
        //public MoneyAccountType AccountType { get; set; }

        //public Guid CategoryId { get { return Guid.Empty; } }

        public decimal MonthlyBudgetAmount { get { return decimal.Zero; } }

        public bool ExcludeFromBudget { get { return false; } }

        public BankAccount()
        {
            Id = Guid.NewGuid();
        }
        public BankAccount(IJournalAccount cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(IJournalAccount cpy)
        {
            if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Copy MUST be a Bank Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            //this.MoneyAccountId = cpy.MoneyAccountId;
            //this.AccountType = cpy.AccountType;
        }

//#pragma warning disable CS0612 // Type or member is obsolete
//        public BankAccount(MoneyAccount old) : this()
//        {
//            this.Convert(old);
//        }


//        public void Convert(MoneyAccount act)
//        {
//            MoneyAccountId = act.ID;
//            Description = act.Description;
//            OrderBy = act.OrderBy;
//            DateClosedUTC = act.DateClosedUTC;
//            AccountType = act.AccountType;
//        }
//#pragma warning restore CS0612 // Type or member is obsolete
    }
}
