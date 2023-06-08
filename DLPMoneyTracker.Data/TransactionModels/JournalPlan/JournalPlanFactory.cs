using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.TransactionModels.JournalPlan
{
    public class JournalPlanFactory
    {
        public static IJournalPlan Build(ITrackerConfig config, JournalPlanJSON json)
        {
            var credit = config.LedgerAccountsList.FirstOrDefault(x => x.Id == json.CreditAccountId);
            var debit = config.LedgerAccountsList.FirstOrDefault(x => x.Id == json.DebitAccountId);
            var recurrence = ScheduleRecurrenceFactory.Build(json.RecurrenceJSON);

            switch(json.PlanType)
            {
                case JournalPlanType.Payable:
                    var payable = new PayablePlan()
                    {
                        UID = json.UID,
                        CreditAccount = credit,
                        DebitAccount = debit,
                        Description = json.Description,
                        Recurrence = recurrence,
                        ExpectedAmount = json.ExpectedAmount
                    };
                    if (!payable.IsValid()) return null;
                    return payable;
                case JournalPlanType.Receivable:
                    var receivable = new ReceivablePlan()
                    {
                        UID = json.UID,
                        CreditAccount = credit,
                        DebitAccount = debit,
                        Description = json.Description,
                        Recurrence = recurrence,
                        ExpectedAmount = json.ExpectedAmount
                    };
                    if(!receivable.IsValid()) return null;
                    return receivable;
                case JournalPlanType.Transfer:
                    var transfer = new TransferPlan()
                    {
                        UID = json.UID,
                        CreditAccount = credit,
                        DebitAccount = debit,
                        Description = json.Description,
                        Recurrence = recurrence,
                        ExpectedAmount = json.ExpectedAmount
                    };
                    if (!transfer.IsValid()) return null;
                    return transfer;
                case JournalPlanType.DebtPayment:
                    var debt = new DebtPaymentPlan()
                    {
                        UID = json.UID,
                        CreditAccount = credit,
                        DebitAccount = debit,
                        Description = json.Description,
                        Recurrence = recurrence,
                        ExpectedAmount = json.ExpectedAmount
                    };
                    if (!debt.IsValid()) return null;
                    return debt;
                default:
                    return null;
            }

        }

        public static IJournalPlan Build(ITrackerConfig config, JournalPlanType pType, string desc, IJournalAccount credit, IJournalAccount debit, decimal amount, IScheduleRecurrence recurrence)
        {
            switch(pType)
            {
                case JournalPlanType.Payable:
                    return new PayablePlan()
                    {
                        Description = desc,
                        CreditAccount = credit,
                        DebitAccount = debit,
                        Recurrence = recurrence,
                        ExpectedAmount = amount
                    };
                case JournalPlanType.Receivable:
                    return new ReceivablePlan()
                    {
                        Description = desc,
                        CreditAccount = credit,
                        DebitAccount = debit,
                        Recurrence = recurrence,
                        ExpectedAmount = amount
                    };
                case JournalPlanType.Transfer:
                    return new TransferPlan()
                    {
                        Description = desc,
                        CreditAccount = credit,
                        DebitAccount = debit,
                        Recurrence = recurrence,
                        ExpectedAmount = amount
                    };
                case JournalPlanType.DebtPayment:
                    return new DebtPaymentPlan()
                    {
                        Description = desc,
                        CreditAccount = credit,
                        DebitAccount = debit,
                        Recurrence = recurrence,
                        ExpectedAmount = amount
                    };
                default:
                    return null;
            }
        }



#pragma warning disable CS0612 // Type or member is obsolete
        public static IJournalPlan Build(ITrackerConfig config, IMoneyPlan plan)
        {
            if (plan is null) return null;

            IJournalAccount moneyAccount = config.LedgerAccountsList.FirstOrDefault(x => x.MoneyAccountId == plan.AccountID);
            IJournalAccount categoryAccount = config.LedgerAccountsList.FirstOrDefault(x => x.CategoryId == plan.CategoryID);

            // If the plans are not Valid it is most likely either a Debt Payment or a Transfer
            // which I'll have to redo by hand
            switch (plan.PlanType)
            {
                case MoneyPlanType.Income:
                    ReceivablePlan receivable = new ReceivablePlan()
                    {
                        DebitAccount = moneyAccount,
                        CreditAccount = categoryAccount,
                        Description = plan.Description,
                        RecurrenceJSON = plan.RecurrenceJSON,
                        ExpectedAmount = plan.ExpectedAmount
                    };
                    if(receivable.IsValid()) return receivable;

                    return null; 
                case MoneyPlanType.Expense:
                    PayablePlan payable = new PayablePlan()
                    {
                        DebitAccount = categoryAccount,
                        CreditAccount = moneyAccount,
                        Description = plan.Description,
                        RecurrenceJSON = plan.RecurrenceJSON,
                        ExpectedAmount = plan.ExpectedAmount
                    };
                    if (payable.IsValid()) return payable;
                    return null;
                default:
                    return null;
            } 
            
        }
#pragma warning restore CS0612 // Type or member is obsolete
    }
}
