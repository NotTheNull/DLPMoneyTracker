using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using DLPMoneyTracker.Core.Models.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.BudgetPlan
{
    
    public interface IBudgetPlan
    {
        Guid UID { get; }
        BudgetPlanType PlanType { get; }
        string Description { get; }
        decimal ExpectedAmount { get; }


        // Debit
        List<LedgerType> ValidDebitAccountTypes { get; }
        Guid DebitAccountId { get; }
        string DebitAccountName { get; }


        // Credit
        List<LedgerType> ValidCreditAccountTypes { get; }
        Guid CreditAccountId { get; }
        string CreditAccountName { get; }

        // Recurrence
        DateTime NotificationDate { get; }
        DateTime NextOccurrence { get; }


        bool IsValid();
    }
}
