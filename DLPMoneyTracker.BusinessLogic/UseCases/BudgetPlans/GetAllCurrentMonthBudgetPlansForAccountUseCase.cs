using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class GetAllCurrentMonthBudgetPlansForAccountUseCase(IBudgetPlanRepository budgetRepository) : IGetAllCurrentMonthBudgetPlansForAccountUseCase
    {
        public List<IBudgetPlan> Execute(Guid accountUID)
        {
            return budgetRepository.GetAllPlansForAccount(accountUID);
        }
    }
}