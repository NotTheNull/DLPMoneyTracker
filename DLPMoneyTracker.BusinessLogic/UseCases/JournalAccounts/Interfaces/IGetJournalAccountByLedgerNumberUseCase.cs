using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface IGetJournalAccountByLedgerNumberUseCase
    {
        IJournalAccount Execute(string idCPA);
    }
}