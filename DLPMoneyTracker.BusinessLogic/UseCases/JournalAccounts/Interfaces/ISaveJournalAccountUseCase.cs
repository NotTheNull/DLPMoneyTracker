using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface ISaveJournalAccountUseCase
    {
        void Execute(IJournalAccount account);
    }
}