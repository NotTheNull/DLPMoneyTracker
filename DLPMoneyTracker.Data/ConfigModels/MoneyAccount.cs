using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DLPMoneyTracker.Data.ConfigModels
{
    public enum MoneyAccountType
    {
        Checking,
        Savings,
        CreditCard,
        Loan,
        NotSet
    }


    public class MoneyAccount
    {
        public string ID { get; set; } 

        public string Description { get; set; }

        public MoneyAccountType AccountType { get; set; }

        public string WebAddress { get; set; }



        public MoneyAccount()
        {
            this.ID = this.Description = this.WebAddress = string.Empty;
            this.AccountType = MoneyAccountType.NotSet;
        }
    }
}
