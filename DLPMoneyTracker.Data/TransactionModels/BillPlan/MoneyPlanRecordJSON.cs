using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    public class MoneyPlanRecordJSON : IMoneyPlan
    {
        public Guid UID { get; set; }

        public string Description { get; set; }

        public Guid CategoryID { get; set; }

        public string AccountID { get; set; }

        public string RecurrenceJSON { get; set; }

        public decimal ExpectedAmount { get; set; }
    }
}
