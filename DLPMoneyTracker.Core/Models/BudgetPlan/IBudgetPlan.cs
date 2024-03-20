using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.BudgetPlan
{
    
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
        IJournalAccount DebitAccount { get; }
        Guid DebitAccountId { get; }
        string DebitAccountName { get; }


        // Credit
        List<LedgerType> ValidCreditAccountTypes { get; }
        IJournalAccount CreditAccount { get; }
        Guid CreditAccountId { get; }
        string CreditAccountName { get; }

        // Recurrence
        IScheduleRecurrence Recurrence { get; }
        DateTime NotificationDate { get; }
        DateTime NextOccurrence { get; }


        bool IsValid();
        void Copy(IBudgetPlan plan);
    }
}
