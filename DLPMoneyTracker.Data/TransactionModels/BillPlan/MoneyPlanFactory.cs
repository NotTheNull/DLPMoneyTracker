using System;
using System.Collections.Generic;
using System.Text;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;

namespace DLPMoneyTracker.Data.TransactionModels.BillPlan
{
    public class MoneyPlanFactory
    {
        public static IMoneyPlan Build(ITrackerConfig config, MoneyPlanRecordJSON jsonRecord)
        {
            TransactionCategory category = config.GetCategory(jsonRecord.CategoryID);
            MoneyAccount account = config.GetAccount(jsonRecord.AccountID);
            IScheduleRecurrence recurr = ScheduleRecurrenceFactory.Build(jsonRecord.RecurrenceJSON);

            return Build(category, account, jsonRecord.Description, recurr, jsonRecord.ExpectedAmount, jsonRecord.UID);
        }

        public static IMoneyPlan Build(TransactionCategory category, MoneyAccount account, string desc, IScheduleRecurrence recurr, decimal amount, Guid? uid = null)
        {
            if (!uid.HasValue) uid = Guid.NewGuid();

            switch(category.CategoryType)
            {
                case CategoryType.Expense:
                    return new ExpensePlan()
                    {
                        UID = uid.Value,
                        Account = account,
                        Category = category,
                        Description = desc,
                        Recurrence = recurr,
                        ExpectedAmount = amount
                    };
                case CategoryType.Income:
                    return new IncomePlan()
                    {
                        UID = uid.Value,
                        Account = account,
                        Category = category,
                        Description = desc,
                        Recurrence = recurr,
                        ExpectedAmount = amount
                    };
                default:
                    return null;
            }
        }
    }
}
