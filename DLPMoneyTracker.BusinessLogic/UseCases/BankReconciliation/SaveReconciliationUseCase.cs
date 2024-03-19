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
    public class SaveReconciliationUseCase : ISaveReconciliationUseCase
    {
        private readonly IBankReconciliationRepository reconciliationRepository;

        public SaveReconciliationUseCase(IBankReconciliationRepository reconciliationRepository)
        {
            this.reconciliationRepository = reconciliationRepository;
        }

        public void Execute(BankReconciliationDTO dto)
        {
            reconciliationRepository.SaveReconciliation(dto);
        }
    }
}
