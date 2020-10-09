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

        string Description { get; }

        Guid CategoryID { get; }

        string AccountID { get; }

        string RecurrenceJSON { get; }

        decimal ExpectedAmount { get; }

    }
}
