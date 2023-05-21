using System;

namespace DLPMoneyTracker.Data.ConfigModels
{
    public enum CategoryType
    {
        Income,
        Expense,
        UntrackedAdjustment,
        NotSet,
        Payment, // For Credit Card and Loans
        InitialBalance,
        TransferFrom,
        TransferTo
    }

    public class TransactionCategory
    {
        public static TransactionCategory InitialBalance { get { return new TransactionCategory() { ID = Guid.Empty, Name = "*STARTING BALANCE*", CategoryType = CategoryType.InitialBalance }; } }
        public static TransactionCategory DebtPayment { get { return new TransactionCategory() { ID = new Guid("11111111-1111-1111-1111-111111111111"), Name = "*DEBT PAYMENT*", CategoryType = CategoryType.Payment }; } }
        public static TransactionCategory TransferFrom { get { return new TransactionCategory() { ID = new Guid("22222222-2222-2222-2222-222222222222"), Name = "*XFER FROM*", CategoryType = CategoryType.TransferFrom }; } }
        public static TransactionCategory TransferTo { get { return new TransactionCategory() { ID = new Guid("22222222-2222-2222-2222-222222222223"), Name = "*XFER TO*", CategoryType = CategoryType.TransferTo }; } }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public CategoryType CategoryType { get; set; }
        public bool ExcludeFromBudget { get; set; } // Determines whether it will be visible on the Budget Planner as well as whether it will affect the Budget totals
        public DateTime? DateDeletedUTC { get; set; } // If set, will no longer be available for NEW transactions
        public decimal DefaultMonthlyBudget { get; set; }

        public TransactionCategory()
        {
            this.ID = Guid.NewGuid();
            this.Name = string.Empty;
            this.CategoryType = CategoryType.NotSet;
            this.DefaultMonthlyBudget = 0m;
        }
        public TransactionCategory(TransactionCategory cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(TransactionCategory src)
        {
            this.Name = src.Name;
            this.CategoryType = src.CategoryType;
            this.ExcludeFromBudget = src.ExcludeFromBudget;
            this.DateDeletedUTC = src.DateDeletedUTC;
            this.DefaultMonthlyBudget = src.DefaultMonthlyBudget;
        }
    }

    /// <summary>
    /// This purpose is to assist with transfer between changes
    /// </summary>
    public class TransactionCategoryJSONTransferVersion
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public CategoryType CategoryType { get; set; }
    }
}