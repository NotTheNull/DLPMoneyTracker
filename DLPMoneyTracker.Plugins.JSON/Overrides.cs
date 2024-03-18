using DLPMoneyTracker.Core.Models.Source;
using DLPMoneyTracker.Plugins.JSON.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON
{
    internal static class Overrides
    {
        public static BudgetPlanType ToBudgetPlanType(this JournalPlanType jType)
        {
            switch(jType)
            {
                case JournalPlanType.DebtPayment:
                    return BudgetPlanType.DebtPayment;
                case JournalPlanType.Payable:
                    return BudgetPlanType.Payable;
                case JournalPlanType.Receivable:
                    return BudgetPlanType.Receivable;
                case JournalPlanType.Transfer:
                    return BudgetPlanType.Transfer;
                default:
                    return BudgetPlanType.NotSet;
            }
        }
    }
}
