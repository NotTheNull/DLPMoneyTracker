using System;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class CreditCardAccount : IJournalAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public JournalAccountType JournalType
        { get { return JournalAccountType.LiabilityCard; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        // For backwards compatibility
        //public string MoneyAccountId { get; set; }
        //public MoneyAccountType AccountType { get { return MoneyAccountType.CreditCard; } }

        //public Guid CategoryId { get { return Guid.Empty; } }

        public decimal MonthlyBudgetAmount
        { get { return decimal.Zero; } }

        public bool ExcludeFromBudget
        { get { return false; } }

        public CreditCardAccount()
        {
            Id = Guid.NewGuid();
        }

        public CreditCardAccount(IJournalAccount cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(IJournalAccount cpy)
        {
            if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Copy MUST be a Credit Card Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            //this.MoneyAccountId = cpy.MoneyAccountId;
        }

        //#pragma warning disable CS0612 // Type or member is obsolete
        //        public CreditCardAccount(MoneyAccount old) : this()

        //        {
        //            this.Convert(old);
        //        }

        //        public void Convert(MoneyAccount act)
        //        {
        //            MoneyAccountId = act.ID;
        //            Description = act.Description;
        //            OrderBy = act.OrderBy;
        //            DateClosedUTC = act.DateClosedUTC;
        //        }
        //#pragma warning restore CS0612 // Type or member is obsolete
    }
}