using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface IGetJournalAccountListByTypesUseCase
    {
        List<IJournalAccount> Execute(List<LedgerType> listTypes);
    }
}