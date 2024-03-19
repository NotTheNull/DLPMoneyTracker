using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.Core.Models.BankReconciliation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation
{
    public class GetBankReconciliationListUseCase : IGetBankReconciliationListUseCase
    {
        private readonly IBankReconciliationRepository reconciliationRepository;

        public GetBankReconciliationListUseCase(IBankReconciliationRepository reconciliationRepository)
        {
            this.reconciliationRepository = reconciliationRepository;
        }

        public List<BankReconciliationOverviewDTO> Execute()
        {
            return reconciliationRepository.GetFullList();
        }
    }
}
