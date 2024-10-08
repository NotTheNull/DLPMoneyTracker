﻿using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    [Obsolete]
    [DebuggerDisplay("INCOME: {AccountName} to {CategoryName}")]
    public class IncomePlan : IMoneyPlan
    {
        public Guid UID { get; set; }

        [JsonIgnore]
        public MoneyPlanType PlanType { get { return MoneyPlanType.Income; } }

        public int PriorityOrder { get { return 1; } }

        private TransactionCategory _cat;

        [JsonIgnore]
        public TransactionCategory Category
        {
            get { return _cat; }
            set
            {
                if (value.CategoryType != CategoryType.Income) throw new InvalidOperationException("Only Income Categories are allowed");
                _cat = value;
            }
        }

        [JsonIgnore]
        public string CategoryName { get { return this.Category?.Name ?? string.Empty; } }

        [JsonIgnore]
        public bool ExcludeFromBudgetPlanner { get { return this.Category?.ExcludeFromBudget ?? false; } }

        public Guid CategoryID { get { return this.Category?.ID ?? Guid.Empty; } }

        [JsonIgnore]
        public MoneyAccount Account { get; set; }

        [JsonIgnore]
        public string AccountName { get { return this.Account?.Description ?? string.Empty; } }

        public string AccountID { get { return this.Account?.ID ?? string.Empty; } }

        public string Description { get; set; }

        [JsonIgnore]
        public IScheduleRecurrence Recurrence { get; set; }

        public string RecurrenceJSON
        {
            get { return this.Recurrence.GetFileData(); }
            set
            {
                this.Recurrence = ScheduleRecurrenceFactory.Build(value);
            }
        }

        [JsonIgnore]
        public RecurrenceFrequency Frequency { get { return this.Recurrence.Frequency; } }

        [JsonIgnore]
        public DateTime NotificationDate { get { return this.Recurrence?.NotificationDate.AddDays(1).AddMilliseconds(-1) ?? DateTime.MinValue; } }

        [JsonIgnore]
        public DateTime NextOccurrence { get { return this.Recurrence?.NextOccurence ?? DateTime.MaxValue; } }

        public decimal ExpectedAmount { get; set; }

        public IncomePlan()
        {
            this.UID = Guid.NewGuid();
        }
    }
}