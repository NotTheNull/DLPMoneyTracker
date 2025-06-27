using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BankReconciliation;

namespace DLPMoneyTracker.BusinessLogic.PluginInterfaces
{
    public interface IBankReconciliationRepository
    {
        List<BankReconciliationOverviewDTO> GetFullList();

        List<IMoneyTransaction> GetReconciliationTransactions(Guid accountUID, DateRange statementDates);

        void SaveReconciliation(BankReconciliationDTO dto);

        int GetRecordCount();
    }
}