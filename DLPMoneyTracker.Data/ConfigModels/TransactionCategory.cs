using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.Data.ConfigModels
{
    public enum CategoryType
    {
        Income,
        Expense,
        UntrackedAdjustment,
        NotSet,
        Payment, // For Credit Card and Loans
        InitialBalance
    }

    public class TransactionCategory
    {
        public static TransactionCategory InitialBalance { get { return new TransactionCategory() { ID = Guid.Empty, Name = "*STARTING BALANCE*", CategoryType = CategoryType.InitialBalance }; } }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public CategoryType CategoryType { get; set; }



        public TransactionCategory()
        {
            this.ID = Guid.NewGuid();
            this.Name = string.Empty;
            this.CategoryType = CategoryType.NotSet;
        }
    }
}
