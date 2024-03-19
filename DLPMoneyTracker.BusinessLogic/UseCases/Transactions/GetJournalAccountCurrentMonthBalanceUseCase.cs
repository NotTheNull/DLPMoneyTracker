using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{

    public class GetJournalAccountCurrentMonthBalanceUseCase : IGetJournalAccountCurrentMonthBalanceUseCase
    {
        private readonly ITransactionRepository moneyRepository;

        public GetJournalAccountCurrentMonthBalanceUseCase(ITransactionRepository moneyRepository)
        {
            this.moneyRepository = moneyRepository;
        }

        public decimal Execute(Guid accountUID)
        {
            /*
             * This should be different depending on the account
             * MONEY ACCOUNTS -> sum of all records
             * NOMINAL ACCOUNTS -> sum of current month records
             * Will need to have a separate YTD use case
             * 
             */
            return moneyRepository.GetAccountBalance(accountUID);
        }
    }
}
