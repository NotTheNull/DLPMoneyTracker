using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces
{
    public interface ISaveBudgetPlanUseCase
    {
        void Execute(IBudgetPlan plan);
    }
}