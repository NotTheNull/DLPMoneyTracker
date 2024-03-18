using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.BudgetPlan
{
    public enum RecurrenceFrequency
    {
        BiWeekly, // every two weeks [26 times a year]
        SemiMonthly, // twice a month [24 times a year]
        Monthly,
        SemiAnnual,
        Annual
    }

    public enum BudgetPlanType
    {
        Payable,
        Receivable,
        Transfer,
        DebtPayment,
        NotSet
    }


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
        IScheduleRecurrence Recurrence { get; }
        DateTime NotificationDate { get; }
        DateTime NextOccurrence { get; }


        bool IsValid();
    }
}
