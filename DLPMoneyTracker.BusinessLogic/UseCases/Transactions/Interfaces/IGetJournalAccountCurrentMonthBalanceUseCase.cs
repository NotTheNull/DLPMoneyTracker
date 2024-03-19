namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces
{
    public interface IGetJournalAccountCurrentMonthBalanceUseCase
    {
        decimal Execute(Guid accountUID);
    }
}