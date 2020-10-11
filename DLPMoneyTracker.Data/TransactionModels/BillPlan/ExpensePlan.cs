using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    public class ExpensePlan : IMoneyPlan
    {
        public Guid UID { get; set; }

        [JsonIgnore]
        public MoneyPlanType PlanType { get { return MoneyPlanType.Expense; } }

        public int PriorityOrder { get { return 2; } }


        private TransactionCategory _cat;

        [JsonIgnore]
        public TransactionCategory Category
        {
            get { return _cat; }
            set 
            {
                if (value.CategoryType != CategoryType.Expense) throw new InvalidOperationException("Only Expense Categories are allowed");
                _cat = value; 
            }
        }


        public Guid CategoryID { get { return this.Category?.ID ?? Guid.Empty; } }

        [JsonIgnore]
        public MoneyAccount Account { get; set; }

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
        public DateTime NotificationDate { get { return this.Recurrence?.NotificationDate ?? DateTime.MinValue; } }

        [JsonIgnore]
        public DateTime NextOccurrence { get { return this.Recurrence?.NextOccurence ?? DateTime.MaxValue; } }

        public decimal ExpectedAmount { get; set; }


        public ExpensePlan()
        {
            this.UID = Guid.NewGuid();
        }
    }
}
