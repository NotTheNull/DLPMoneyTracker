using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.Data.TransactionModels
{
    

    public interface IMoneyRecord
    {
        DateTime TransDate { get; }
        MoneyAccount Account { get; }
        string TransType { get; }
        string Description { get; }
        decimal TransAmount { get; }

    }





}
