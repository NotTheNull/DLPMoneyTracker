using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces
{
    public interface IGetBudgetPlanListUseCase
    {
        List<IBudgetPlan> Execute();
    }
}