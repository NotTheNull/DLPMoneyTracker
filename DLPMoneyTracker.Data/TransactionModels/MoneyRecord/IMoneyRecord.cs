using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

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
