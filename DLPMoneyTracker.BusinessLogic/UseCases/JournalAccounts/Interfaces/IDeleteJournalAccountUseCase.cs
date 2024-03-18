namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface IDeleteJournalAccountUseCase
    {
        void Execute(Guid accountUID);
    }
}