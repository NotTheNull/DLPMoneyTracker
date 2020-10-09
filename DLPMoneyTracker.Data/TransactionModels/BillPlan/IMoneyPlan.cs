using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    public interface IMoneyPlan
    {
        Guid UID { get; }

        [JsonIgnore]
        int PriorityOrder { get; } // Mainly for sorting purposes 

        string Description { get; }

        Guid CategoryID { get; }

        string AccountID { get; }

        string RecurrenceJSON { get; }

        decimal ExpectedAmount { get; }

        [JsonIgnore]
        DateTime NotificationDate { get; }

        [JsonIgnore]
        DateTime NextOccurrence { get; }

    }

    public class MoneyPlanRecordJSON : IMoneyPlan
    {
        public Guid UID { get; set; }

        [JsonIgnore]
        public int PriorityOrder { get { return 9999999; } } 

        public string Description { get; set; }

        public Guid CategoryID { get; set; }

        public string AccountID { get; set; }

        public string RecurrenceJSON { get; set; }

        public decimal ExpectedAmount { get; set; }

        [JsonIgnore]
        public DateTime NotificationDate { get { return DateTime.MinValue; } }

        [JsonIgnore]
        public DateTime NextOccurrence { get { return DateTime.MaxValue; } }
    }
}
