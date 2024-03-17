using System;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class PayableAccount : IJournalAccount, ILedgerAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType
        { get { return LedgerType.Payable; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        //public string MoneyAccountId { get { return string.Empty; } }
        //public MoneyAccountType AccountType { get { return MoneyAccountType.NotSet; } }
        //public Guid CategoryId { get; set; }

        // Exclusive Properties
        public decimal MonthlyBudgetAmount { get; set; }

        public bool ExcludeFromBudget { get; set; }

        public PayableAccount()
        {
            Id = Guid.NewGuid();
        }

        public PayableAccount(IJournalAccount cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(IJournalAccount cpy)
        {
            if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Copy MUST be a Payable Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            
            if(cpy is ILedgerAccount ledger)
            {
                this.MonthlyBudgetAmount = ledger.MonthlyBudgetAmount;
            }
        }

        //#pragma warning disable CS0612 // Type or member is obsolete
        //        public PayableAccount(TransactionCategory old) : this()
        //        {
        //            this.Convert(old);
        //        }

        //        public void Convert(TransactionCategory cat)
        //        {
        //            CategoryId = cat.ID;
        //            Description = cat.Name;
        //            DateClosedUTC = cat.DateDeletedUTC;
        //            this.ExcludeFromBudget = cat.ExcludeFromBudget;
        //            MonthlyBudgetAmount = cat.DefaultMonthlyBudget;
        //        }
        //#pragma warning restore CS0612 // Type or member is obsolete
    }
}