using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class GetJournalAccountCurrentMonthBalanceUseCase(ITransactionRepository moneyRepository) : IGetJournalAccountCurrentMonthBalanceUseCase
    {
        public decimal Execute(Guid accountUID)
        {
            /*
             * This should be different depending on the account
             * MONEY ACCOUNTS -> sum of all records
             * NOMINAL ACCOUNTS -> sum of current month records
             * Will need to have a separate YTD use case
             *
             */

            return moneyRepository.GetCurrentAccountBalance(accountUID);
        }
    }
}