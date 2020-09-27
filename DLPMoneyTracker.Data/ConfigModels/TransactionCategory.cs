using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

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
        public static TransactionCategory DebtPayment { get { return new TransactionCategory() { ID = new Guid("11111111-1111-1111-1111-111111111111"), Name = "*DEBT PAYMENT*", CategoryType = CategoryType.Payment }; } }

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
