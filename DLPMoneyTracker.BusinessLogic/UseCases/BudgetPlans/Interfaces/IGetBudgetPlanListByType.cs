using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces
{
    public interface IGetBudgetPlanListByType
    {
        List<IBudgetPlan> Execute(BudgetPlanType planType);
    }
}