using System;
using System.Diagnostics;

namespace DLPMoneyTracker.Data.TransactionModels
{
    [DebuggerDisplay("{AccountID} {Description} ${TransAmount}")]
    public class MoneyRecordJSON : IMoneyRecord
    {
        public Guid TransactionId { get; set; }
        public DateTime TransDate { get; set; }

        public string AccountID { get; set; }

        public Guid CategoryUID { get; set; }

        public string Description { get; set; }

        public decimal TransAmount { get; set; }
    }
}