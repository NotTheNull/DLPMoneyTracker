using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface IGetLedgerAccountsUseCase
    {
        List<IJournalAccount> Execute(bool includeDeleted);
    }
}