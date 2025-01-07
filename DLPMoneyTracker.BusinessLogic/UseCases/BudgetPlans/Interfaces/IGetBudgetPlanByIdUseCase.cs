using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces
{
    public interface IGetBudgetPlanByIdUseCase
    {
        IBudgetPlan Execute(Guid uid);
    }
}