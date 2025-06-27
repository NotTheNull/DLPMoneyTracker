using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans
{
    public class DeleteBudgetPlanUseCase(IBudgetPlanRepository budgetRepository) : IDeleteBudgetPlanUseCase
    {
        public void Execute(Guid planUID)
        {
            budgetRepository.DeletePlan(planUID);
        }
    }
}