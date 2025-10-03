using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans;

public class GetUpcomingPlansForAccountUseCase(IBudgetPlanRepository budgetRepository) : IGetUpcomingPlansForAccountUseCase
{
    public List<IBudgetPlan> Execute(Guid accountUID)
    {
        return budgetRepository.GetUpcomingPlansForAccount(accountUID);
    }
}