using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.Factories
{
    public class BudgetPlanFactory
    {   

        public IBudgetPlan Build(IBudgetPlan plan)
        {
            return Build(plan.PlanType, plan.UID, plan.Description, plan.DebitAccount, plan.CreditAccount, plan.ExpectedAmount, plan.Recurrence);
        }

        public IBudgetPlan Build(BudgetPlanType planType, Guid uid, string desc, IJournalAccount debit, IJournalAccount credit, decimal amount, IScheduleRecurrence recurrence)
        {
            if (uid == Guid.Empty) uid = Guid.NewGuid();

            switch(planType)
            {
                case BudgetPlanType.Receivable:
                    return new ReceivablePlan()
                    {
                        UID = uid,
                        Description = desc,
                        DebitAccount = debit,
                        CreditAccount = credit,
                        Recurrence = recurrence,
                        ExpectedAmount = amount
                    };
                case BudgetPlanType.Payable:
                    return new PayablePlan()
                    {
                        UID = uid,
                        Description = desc,
                        DebitAccount = debit,
                        CreditAccount = credit,
                        Recurrence = recurrence,
                        ExpectedAmount = amount
                    };
                case BudgetPlanType.DebtPayment:
                    return new DebtPaymentPlan()
                    {
                        UID = uid,
                        Description = desc,
                        DebitAccount = debit,
                        CreditAccount = credit,
                        Recurrence = recurrence,
                        ExpectedAmount = amount
                    };
                case BudgetPlanType.Transfer:
                    return new TransferPlan()
                    {
                        UID = uid,
                        Description = desc,
                        DebitAccount = debit,
                        CreditAccount = credit,
                        Recurrence = recurrence,
                        ExpectedAmount = amount
                    };
                default:
                    throw new InvalidOperationException($"Type [{planType}] is not currently supported");
            }

        }



    }
}
