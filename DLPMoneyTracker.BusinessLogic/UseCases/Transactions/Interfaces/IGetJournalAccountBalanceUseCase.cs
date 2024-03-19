namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces
{
    public interface IGetJournalAccountBalanceUseCase
    {
        decimal Execute(Guid accountUID);
    }
}