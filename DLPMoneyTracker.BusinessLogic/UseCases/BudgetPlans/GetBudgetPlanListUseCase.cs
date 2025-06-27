using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class GetBudgetPlanListUseCase(IBudgetPlanRepository budgetRepository) : IGetBudgetPlanListUseCase
    {
        public List<IBudgetPlan> Execute()
        {
            return budgetRepository.GetFullList();
        }
    }
}