using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface IGetNextSubLedgerIdUseCase
    {
        int Execute(LedgerType accountType, int idCategory);
    }
}