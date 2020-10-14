using System;

namespace DLPMoneyTracker.Data.TransactionModels
{
    public interface IMoneyRecord
    {
        Guid TransactionId { get; }
        DateTime TransDate { get; }
        string AccountID { get; }
        Guid CategoryUID { get; }
        string Description { get; }
        decimal TransAmount { get; }
    }
}