using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.Core.Models.BankReconciliation;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation
{
    public class SaveReconciliationUseCase(IBankReconciliationRepository reconciliationRepository) : ISaveReconciliationUseCase
    {
        public void Execute(BankReconciliationDTO dto)
        {
            reconciliationRepository.SaveReconciliation(dto);
        }
    }
}