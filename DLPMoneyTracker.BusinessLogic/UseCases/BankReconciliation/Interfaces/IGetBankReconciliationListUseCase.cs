using DLPMoneyTracker.Core.Models.BankReconciliation;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces
{
    public interface IGetBankReconciliationListUseCase
    {
        List<BankReconciliationOverviewDTO> Execute();
    }
}