using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class GetJournalAccountYTDUseCase(ITransactionRepository moneyRepository) : IGetJournalAccountYTDUseCase
    {
        public decimal Execute(Guid accountUID, int year)
        {
            return moneyRepository.GetAccountBalanceYTD(accountUID, year);
        }
    }
}