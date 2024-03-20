using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface IGetNominalAccountsUseCase
    {
        List<IJournalAccount> Execute(bool includeDeleted);
    }
}