using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    public class BudgetRecordJSON : IBudgetRecord
    {
        public Guid UID { get; set; }

        public string BillDescription { get; set; }

        public Guid CategoryID { get; set; }

        public string RecurrenceJSON { get; set; }

        public decimal ExpectedAmount { get; set; }
    }
}
