using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface IGetJournalAccountByUIDUseCase
    {
        IJournalAccount Execute(Guid uid);
    }
}