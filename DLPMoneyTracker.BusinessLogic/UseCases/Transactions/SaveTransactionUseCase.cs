using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.Transactions
{
    public class SaveTransactionUseCase : ISaveTransactionUseCase
    {
        private readonly ITransactionRepository moneyRepository;

        public SaveTransactionUseCase(ITransactionRepository moneyRepository)
        {
            this.moneyRepository = moneyRepository;
        }

        public void Execute(IMoneyTransaction transaction)
        {
            moneyRepository.SaveTransaction(transaction);
        }
    }
}
