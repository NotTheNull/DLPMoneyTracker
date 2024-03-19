using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces
{
    public interface IGetTransactionsBySearchUseCase
    {
        List<IMoneyTransaction> Execute(DateRange? dateRange, string? filterText, IJournalAccount? account);
    }
}