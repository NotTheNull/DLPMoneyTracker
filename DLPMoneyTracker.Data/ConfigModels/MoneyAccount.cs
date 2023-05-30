using System;
using System.Diagnostics;
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
    [Obsolete]
    public class MoneyAccount
    {
        public string ID { get; set; }

        public string Description { get; set; }

        public MoneyAccountType AccountType { get; set; }

        public string WebAddress { get; set; }

        public DateTime? DateClosedUTC { get; set; } // If set, Money Account will no longer be displayed on Main tab

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
        public MoneyAccount(MoneyAccount cpy)
        {
            this.ID = cpy.ID;
            this.Copy(cpy);
        }

        public void Copy(MoneyAccount moneyAccount)
        {
            this.Description = moneyAccount.Description;
            this.AccountType = moneyAccount.AccountType;
            this.WebAddress = moneyAccount.WebAddress;
            this.DateClosedUTC = moneyAccount.DateClosedUTC;
        }
    }
}