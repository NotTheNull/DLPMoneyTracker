using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    public interface IBudgetRecord
    {
        Guid UID { get; }

        string BillDescription { get; }

        Guid CategoryID { get; }

        string RecurrenceJSON { get; }

        decimal ExpectedAmount { get; }

    }
}
