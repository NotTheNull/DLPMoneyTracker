using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class SaveBudgetPlanUseCase(IBudgetPlanRepository budgetRepository) : ISaveBudgetPlanUseCase
    {
        public void Execute(IBudgetPlan plan)
        {
            if (plan.DebitAccount == SpecialAccount.InvalidAccount || plan.CreditAccount == SpecialAccount.InvalidAccount)
                throw new InvalidOperationException("You must use a valid account");

            budgetRepository.SavePlan(plan);
        }
    }
}