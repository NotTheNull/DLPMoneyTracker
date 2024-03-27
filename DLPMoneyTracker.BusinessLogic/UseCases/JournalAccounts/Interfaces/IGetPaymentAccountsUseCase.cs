using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface IGetPaymentAccountsUseCase
    {
        List<IJournalAccount> Execute(bool includeDeleted = false);
    }
}