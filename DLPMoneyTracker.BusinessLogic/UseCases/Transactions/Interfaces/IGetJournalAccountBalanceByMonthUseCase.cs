namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces
{
    public interface IGetJournalAccountBalanceByMonthUseCase
    {
        decimal Execute(Guid accountUID, int year, int month);
    }
}