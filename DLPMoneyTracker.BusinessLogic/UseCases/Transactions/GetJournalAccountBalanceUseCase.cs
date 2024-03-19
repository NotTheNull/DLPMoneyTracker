using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class GetJournalAccountBalanceUseCase : IGetJournalAccountBalanceUseCase
    {
        private readonly ITransactionRepository moneyRepository;

        public GetJournalAccountBalanceUseCase(ITransactionRepository moneyRepository)
        {
            this.moneyRepository = moneyRepository;
        }

        public decimal Execute(Guid accountUID)
        {
            return moneyRepository.GetAccountBalance(accountUID);
        }
    }
}
