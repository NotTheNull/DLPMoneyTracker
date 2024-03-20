using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class GetJournalAccountYTDUseCase : IGetJournalAccountYTDUseCase
    {
        private readonly ITransactionRepository moneyRepository;

        public GetJournalAccountYTDUseCase(ITransactionRepository moneyRepository)
        {
            this.moneyRepository = moneyRepository;
        }

        public decimal Execute(Guid accountUID, int year)
        {
            return moneyRepository.GetAccountBalanceYTD(accountUID, year);
        }
    }
}
