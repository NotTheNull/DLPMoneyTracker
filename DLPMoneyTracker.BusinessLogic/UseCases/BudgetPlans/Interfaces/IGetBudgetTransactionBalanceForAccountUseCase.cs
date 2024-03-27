namespace DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces
{
    public interface IGetBudgetTransactionBalanceForAccountUseCase
    {
        decimal Execute(Guid accountUID);
    }
}