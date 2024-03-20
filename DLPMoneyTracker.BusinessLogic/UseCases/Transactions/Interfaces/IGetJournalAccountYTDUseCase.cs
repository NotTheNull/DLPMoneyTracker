namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces
{
    public interface IGetJournalAccountYTDUseCase
    {
        decimal Execute(Guid accountUID, int year);
    }
}