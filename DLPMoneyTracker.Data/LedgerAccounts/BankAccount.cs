using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class BankAccount : ILedgerAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerTypes LedgerType { get { return LedgerTypes.Bank; } }
        public int OrderBy { get; set; }
        public DateTime? DateClosedUTC { get; set; }

        // For backwards compatibility
        
        public string MoneyAccountId { get; set; }
        
        public MoneyAccountType AccountType { get; set; }

        public Guid CategoryId { get { return Guid.Empty; } }

        public BankAccount()
        {
            Id = Guid.NewGuid();
        }
        public BankAccount(ILedgerAccount cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(ILedgerAccount cpy)
        {
            if (cpy.LedgerType != this.LedgerType) throw new InvalidOperationException("Copy MUST be a Bank Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            this.MoneyAccountId = cpy.MoneyAccountId;
            this.AccountType = cpy.AccountType;
        }

#pragma warning disable CS0612 // Type or member is obsolete
        public BankAccount(MoneyAccount old) : this()
        {
            this.Convert(old);
        }


        public void Convert(MoneyAccount act)
        {
            MoneyAccountId = act.ID;
            Description = act.Description;
            OrderBy = act.OrderBy;
            DateClosedUTC = act.DateClosedUTC;
            AccountType = act.AccountType;
        }
#pragma warning restore CS0612 // Type or member is obsolete
    }
}
