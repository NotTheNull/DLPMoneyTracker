using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface IGetMoneyAccountsUseCase
    {
        List<IJournalAccount> Execute(bool includeDeleted);
    }
}