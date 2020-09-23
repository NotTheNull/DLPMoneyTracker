using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels
{
    
    public interface IMoneyRecord
    {

        DateTime TransDate { get; }
        string AccountID { get; }
        Guid CategoryUID { get; }
        string Description { get; }
        decimal TransAmount { get; }

    }


    public class MoneyRecord : IMoneyRecord
    {
        public DateTime TransDate { get; set; }
        [JsonIgnore]
        public MoneyAccount Account { get; set; }
        public string AccountID { get { return this.Account?.ID ?? string.Empty; } }

        [JsonIgnore]
        public TransactionCategory Category { get; set; }
        public Guid CategoryUID { get { return this.Category?.ID ?? Guid.Empty; } }

        public string Description { get; set; }
        public decimal TransAmount { get; set; }

        public MoneyRecord()
        {
            this.TransDate = DateTime.Now;
            this.Description = string.Empty;
            this.TransAmount = decimal.Zero;
        }

    }


}
