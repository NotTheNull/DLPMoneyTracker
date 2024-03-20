using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class GetJournalAccountBalanceByMonthUseCase : IGetJournalAccountBalanceByMonthUseCase
    {
        private readonly ITransactionRepository moneyRepository;

        public GetJournalAccountBalanceByMonthUseCase(ITransactionRepository moneyRepository)
        {
            this.moneyRepository = moneyRepository;
        }

        public decimal Execute(Guid accountUID, int year, int month)
        {
            return moneyRepository.GetAccountBalanceByMonth(accountUID, year, month);
        }
    }
}
