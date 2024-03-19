using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BankReconciliation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.PluginInterfaces
{
    public interface IBankReconciliationRepository
    {
        List<BankReconciliationOverviewDTO> GetFullList();
        List<IMoneyTransaction> GetReconciliationTransactions(Guid accountUID, DateRange statementDates);
        void SaveReconciliation(BankReconciliationDTO dto);
    }
}
