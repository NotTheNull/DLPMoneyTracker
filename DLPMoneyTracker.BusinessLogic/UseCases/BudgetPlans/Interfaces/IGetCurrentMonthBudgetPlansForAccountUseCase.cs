using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces
{
    public interface IGetCurrentMonthBudgetPlansForAccountUseCase
    {
        List<IBudgetPlan> Execute(Guid accountUID);
    }
}