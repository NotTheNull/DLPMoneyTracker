using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class CreditCardAccount : ILedgerAccount, IMoneyAccountReference
    {
        public Guid Id { get; private set; }

        public string Description { get; set; }

        public LedgerTypes LedgerType { get { return LedgerTypes.LiabilityCard; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }


        // For backwards compatibility
        public string MoneyAccountId { get; set; }
        public MoneyAccountType AccountType { get { return MoneyAccountType.CreditCard; } }



        public CreditCardAccount()
        {
            Id = Guid.NewGuid();
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
