using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces
{
    public interface IGetUpcomingPlansForAccountUseCase
    {
        List<IBudgetPlan> Execute(Guid accountUID);
    }
}