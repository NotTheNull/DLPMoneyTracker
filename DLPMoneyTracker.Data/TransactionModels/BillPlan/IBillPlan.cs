using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    public interface IBillPlan
    {
        Guid UID { get; }

        string BillDescription { get; }

        [JsonIgnore]
        IScheduleRecurrence Recurrence { get; }

        string RecurrenceJSON { get; }

        decimal ExpectedAmount { get; }

    }
}
