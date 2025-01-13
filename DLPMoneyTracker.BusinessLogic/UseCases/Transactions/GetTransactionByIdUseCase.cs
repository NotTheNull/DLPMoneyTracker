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
    public class GetTransactionByIdUseCase : IGetTransactionByIdUseCase
    {
        private readonly ITransactionRepository repoTransaction;

        public GetTransactionByIdUseCase(ITransactionRepository repoTransaction)
        {
            this.repoTransaction = repoTransaction;
        }

        public IMoneyTransaction Execute(Guid uid)
        {
            return repoTransaction.GetTransactionById(uid);
        }
    }
}
