using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

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

    [DebuggerDisplay("{ID} {Description}")]
    public class MoneyAccount
    {
        public string ID { get; set; } 

        public string Description { get; set; }

        public MoneyAccountType AccountType { get; set; }

        public string WebAddress { get; set; }

        [JsonIgnore]
        public int OrderBy
        {
            get
            {
                switch (AccountType)
                {
                    case MoneyAccountType.Checking: return 1;
                    case MoneyAccountType.CreditCard: return 2;
                    case MoneyAccountType.Savings: return 3;
                    case MoneyAccountType.Loan: return 4;
                    default: return 99;
                }
            }
        }

        public MoneyAccount()
        {
            this.ID = this.Description = this.WebAddress = string.Empty;
            this.AccountType = MoneyAccountType.NotSet;
        }
    }
}
