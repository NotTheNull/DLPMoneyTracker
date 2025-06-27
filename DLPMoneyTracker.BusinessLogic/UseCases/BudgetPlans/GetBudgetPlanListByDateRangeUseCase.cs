using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class GetBudgetPlanListByDateRangeUseCase(IBudgetPlanRepository budgetRepository) : IGetBudgetPlanListByDateRangeUseCase
    {
        public List<IBudgetPlan> Execute(DateRange dateRange)
        {
            BudgetPlanSearch search = new()
            {
                DateRange = dateRange
            };

            return budgetRepository.Search(search);
        }
    }
}