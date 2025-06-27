using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class GetTransactionsBySearchUseCase(ITransactionRepository moneyRepository) : IGetTransactionsBySearchUseCase
    {
        public List<IMoneyTransaction> Execute(DateRange? dateRange, string? filterText, IJournalAccount? account)
        {
            MoneyRecordSearch search = new MoneyRecordSearch();
            if (dateRange is null)
            {
                search.DateRange = new DateRange(Common.MINIMUM_DATE, DateTime.Today);
            }
            else
            {
                search.DateRange = dateRange;
            }

            if (!string.IsNullOrWhiteSpace(filterText))
            {
                search.SearchText = filterText;
            }

            if (account != null)
            {
                search.Account = account;
            }

            return moneyRepository.Search(search);
        }
    }
}