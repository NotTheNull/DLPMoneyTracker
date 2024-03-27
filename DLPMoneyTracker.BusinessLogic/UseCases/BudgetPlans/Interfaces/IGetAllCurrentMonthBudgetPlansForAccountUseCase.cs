using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces
{
    public interface IGetAllCurrentMonthBudgetPlansForAccountUseCase
    {
        List<IBudgetPlan> Execute(Guid accountUID);
    }
}