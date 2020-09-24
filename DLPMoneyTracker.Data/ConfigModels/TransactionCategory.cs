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
        NotSet
    }

    public class TransactionCategory
    {
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
