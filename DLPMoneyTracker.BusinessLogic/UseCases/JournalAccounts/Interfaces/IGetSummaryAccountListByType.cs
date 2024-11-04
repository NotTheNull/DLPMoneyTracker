using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces
{
    public interface IGetSummaryAccountListByType
    {
        List<IJournalAccount> Execute(LedgerType accountType);
    }
}