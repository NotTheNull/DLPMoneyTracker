using DLPMoneyTracker.Core.Models.BankReconciliation;

namespace DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces
{
    public interface ISaveReconciliationUseCase
    {
        void Execute(BankReconciliationDTO dto);
    }
}