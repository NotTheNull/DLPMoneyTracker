using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    public class BudgetRecord : IBudgetRecord
    {
        public Guid UID { get; set; }

        [JsonIgnore]
        public TransactionCategory Category { get; set; }

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

        public decimal ExpectedAmount { get; set; }


        public BudgetRecord()
        {
            this.UID = Guid.NewGuid();
        }
    }
}
