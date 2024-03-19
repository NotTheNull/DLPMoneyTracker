using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class GetTransactionsBySearchUseCase : IGetTransactionsBySearchUseCase
    {
        private readonly ITransactionRepository moneyRepository;

        public GetTransactionsBySearchUseCase(ITransactionRepository moneyRepository)
        {
            this.moneyRepository = moneyRepository;
        }

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
