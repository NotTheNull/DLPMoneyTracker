using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation
{
    public class GetReconciliationTransactionsUseCase : IGetReconciliationTransactionsUseCase
    {
        private readonly IBankReconciliationRepository reconciliationRepository;

        public GetReconciliationTransactionsUseCase(IBankReconciliationRepository reconciliationRepository)
        {
            this.reconciliationRepository = reconciliationRepository;
        }

        public List<IMoneyTransaction> Execute(Guid accountUID, DateRange statementDates)
        {
            return reconciliationRepository.GetReconciliationTransactions(accountUID, statementDates);
        }
    }
}
