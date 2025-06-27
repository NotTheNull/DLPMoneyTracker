using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;

namespace DLPMoneyTracker.BusinessLogic.Factories
{
    public static class BudgetPlanFactory
    {
        public static IBudgetPlan Build(IBudgetPlan plan)
        {
            return Build(plan.PlanType, plan.UID, plan.Description, plan.DebitAccount, plan.CreditAccount, plan.ExpectedAmount, plan.Recurrence);
        }

        public static IBudgetPlan Build(BudgetPlanType planType, Guid uid, string desc, IJournalAccount debit, IJournalAccount credit, decimal amount, IScheduleRecurrence recurrence)
        {
            if (uid == Guid.Empty) uid = Guid.NewGuid();

            return planType switch
            {
                BudgetPlanType.Receivable => new ReceivablePlan()
                {
                    UID = uid,
                    Description = desc,
                    DebitAccount = debit,
                    CreditAccount = credit,
                    Recurrence = recurrence,
                    ExpectedAmount = amount
                },
                BudgetPlanType.Payable => new PayablePlan()
                {
                    UID = uid,
                    Description = desc,
                    DebitAccount = debit,
                    CreditAccount = credit,
                    Recurrence = recurrence,
                    ExpectedAmount = amount
                },
                BudgetPlanType.DebtPayment => new DebtPaymentPlan()
                {
                    UID = uid,
                    Description = desc,
                    DebitAccount = debit,
                    CreditAccount = credit,
                    Recurrence = recurrence,
                    ExpectedAmount = amount
                },
                BudgetPlanType.Transfer => new TransferPlan()
                {
                    UID = uid,
                    Description = desc,
                    DebitAccount = debit,
                    CreditAccount = credit,
                    Recurrence = recurrence,
                    ExpectedAmount = amount
                },
                _ => throw new InvalidOperationException($"Type [{planType}] is not currently supported"),
            };
        }
    }
}