namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces
{
    public interface IDeleteBudgetPlanUseCase
    {
        void Execute(Guid planUID);
    }
}