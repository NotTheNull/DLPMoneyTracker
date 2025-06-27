using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class GetJournalAccountBalanceByMonthUseCase(ITransactionRepository moneyRepository) : IGetJournalAccountBalanceByMonthUseCase
    {
        public decimal Execute(Guid accountUID, int year, int month)
        {
            return moneyRepository.GetAccountBalanceByMonth(accountUID, year, month);
        }
    }
}