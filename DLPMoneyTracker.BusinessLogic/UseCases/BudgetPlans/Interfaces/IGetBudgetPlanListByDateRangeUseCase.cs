using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces
{
    public interface IGetBudgetPlanListByDateRangeUseCase
    {
        List<IBudgetPlan> Execute(DateRange dateRange);
    }
}