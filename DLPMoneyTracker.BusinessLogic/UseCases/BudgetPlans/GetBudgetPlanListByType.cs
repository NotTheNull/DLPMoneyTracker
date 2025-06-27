using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class GetBudgetPlanListByType(IBudgetPlanRepository budgetRepository) : IGetBudgetPlanListByType
    {
        public List<IBudgetPlan> Execute(BudgetPlanType planType)
        {
            return budgetRepository.GetPlanListByType(planType);
        }
    }
}