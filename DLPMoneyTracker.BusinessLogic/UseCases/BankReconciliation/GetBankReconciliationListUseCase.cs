using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.Core.Models.BankReconciliation;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation
{
    public class GetBankReconciliationListUseCase(IBankReconciliationRepository reconciliationRepository) : IGetBankReconciliationListUseCase
    {
        public List<BankReconciliationOverviewDTO> Execute()
        {
            return reconciliationRepository.GetFullList();
        }
    }
}