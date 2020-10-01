using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    public class BillPlan : IBillPlan
    {
        public Guid UID { get; set; }

        public string BillDescription { get; set; }

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

        public BillPlan()
        {
            this.UID = Guid.NewGuid();
        }
    }
}
