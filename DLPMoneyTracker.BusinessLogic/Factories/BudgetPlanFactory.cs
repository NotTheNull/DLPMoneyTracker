using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.Factories
{
    public class BudgetPlanFactory
    {
        private ISourceToBudgetPlanAdapter adapter;
        private readonly ILedgerAccountRepository accountRepository;

        public BudgetPlanFactory(ISourceToBudgetPlanAdapter adapter, ILedgerAccountRepository accountRepository)
        {
            this.adapter = adapter;
            this.accountRepository = accountRepository;
        }

        public IBudgetPlan Build(BudgetPlan source)
        {
            adapter.ImportSource(source);
            return Build(adapter);
        }

        public IBudgetPlan Build(IBudgetPlan data)
        {
            switch(data.PlanType)
            {
                case Core.Models.Source.BudgetPlanType.Receivable:
                    return new ReceivablePlan()
                    {
                        UID = data.UID,
                        Description = data.Description,
                        DebitAccount = accountRepository.GetAccountByUID(data.DebitAccountId),
                        CreditAccount = accountRepository.GetAccountByUID(data.CreditAccountId),
                        Recurrence = data.Recurrence,
                        ExpectedAmount = data.ExpectedAmount
                    };
                case Core.Models.Source.BudgetPlanType.Payable:
                    return new PayablePlan()
                    {
                        UID = data.UID,
                        Description = data.Description,
                        DebitAccount = accountRepository.GetAccountByUID(data.DebitAccountId),
                        CreditAccount = accountRepository.GetAccountByUID(data.CreditAccountId),
                        Recurrence = data.Recurrence,
                        ExpectedAmount = data.ExpectedAmount
                    };
                case Core.Models.Source.BudgetPlanType.DebtPayment:
                    return new DebtPaymentPlan()
                    {
                        UID = data.UID,
                        Description = data.Description,
                        DebitAccount = accountRepository.GetAccountByUID(data.DebitAccountId),
                        CreditAccount = accountRepository.GetAccountByUID(data.CreditAccountId),
                        Recurrence = data.Recurrence,
                        ExpectedAmount = data.ExpectedAmount
                    };
                case Core.Models.Source.BudgetPlanType.Transfer:
                    return new TransferPlan()
                    {
                        UID = data.UID,
                        Description = data.Description,
                        DebitAccount = accountRepository.GetAccountByUID(data.DebitAccountId),
                        CreditAccount = accountRepository.GetAccountByUID(data.CreditAccountId),
                        Recurrence = data.Recurrence,
                        ExpectedAmount = data.ExpectedAmount
                    };
                default:
                    throw new InvalidOperationException($"Type [{data.PlanType}] is not currently supported");
            }

        }



    }
}
