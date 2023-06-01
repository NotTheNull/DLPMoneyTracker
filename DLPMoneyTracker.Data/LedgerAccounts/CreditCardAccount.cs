using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class CreditCardAccount : ILedgerAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerTypes LedgerType { get { return LedgerTypes.LiabilityCard; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }


        // For backwards compatibility
        public string MoneyAccountId { get; set; }
        public MoneyAccountType AccountType { get { return MoneyAccountType.CreditCard; } }

        public Guid CategoryId { get { return Guid.Empty; } }

        public CreditCardAccount()
        {
            Id = Guid.NewGuid();
        }
        public CreditCardAccount(ILedgerAccount cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(ILedgerAccount cpy)
        {
            if (cpy.LedgerType != this.LedgerType) throw new InvalidOperationException("Copy MUST be a Credit Card Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            this.MoneyAccountId = cpy.MoneyAccountId;
            
        }


#pragma warning disable CS0612 // Type or member is obsolete
        public CreditCardAccount(MoneyAccount old) : this()

        {
            this.Convert(old);
        }

        public void Convert(MoneyAccount act)
        {
            MoneyAccountId = act.ID;
            Description = act.Description;
            OrderBy = act.OrderBy;
            DateClosedUTC = act.DateClosedUTC;
        }
#pragma warning restore CS0612 // Type or member is obsolete
    }
}
