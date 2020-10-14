using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    public enum MoneyPlanType
    {
        Expense,
        Income,
        NotSet
    }

    public interface IMoneyPlan
    {
        Guid UID { get; }

        [JsonIgnore]
        MoneyPlanType PlanType { get; }

        [JsonIgnore]
        int PriorityOrder { get; } // Mainly for sorting purposes

        string Description { get; }

        Guid CategoryID { get; }

        [JsonIgnore]
        string CategoryName { get; }

        [JsonIgnore]
        bool ExcludeFromBudgetPlanner { get; }

        string AccountID { get; }

        string RecurrenceJSON { get; }

        [JsonIgnore]
        RecurrenceFrequency Frequency { get; }

        decimal ExpectedAmount { get; }

        [JsonIgnore]
        DateTime NotificationDate { get; }

        [JsonIgnore]
        DateTime NextOccurrence { get; }
    }

    public class MoneyPlanRecordJSON : IMoneyPlan
    {
        public Guid UID { get; set; }

        public MoneyPlanType PlanType { get { return MoneyPlanType.NotSet; } }

        [JsonIgnore]
        public int PriorityOrder { get { return 9999999; } }

        public string Description { get; set; }

        public Guid CategoryID { get; set; }

        public string AccountID { get; set; }

        public string RecurrenceJSON { get; set; }

        [JsonIgnore]
        public RecurrenceFrequency Frequency { get { return RecurrenceFrequency.Annual; } }

        public decimal ExpectedAmount { get; set; }

        [JsonIgnore]
        public DateTime NotificationDate { get { return DateTime.MinValue; } }

        [JsonIgnore]
        public DateTime NextOccurrence { get { return DateTime.MaxValue; } }

        [JsonIgnore]
        public string CategoryName { get; set; }

        [JsonIgnore]
        public bool ExcludeFromBudgetPlanner { get; set; }
    }
}