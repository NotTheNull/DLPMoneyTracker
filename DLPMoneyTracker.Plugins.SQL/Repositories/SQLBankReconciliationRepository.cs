using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BankReconciliation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    public class SQLBankReconciliationRepository : IBankReconciliationRepository
    {
        public List<BankReconciliationOverviewDTO> GetFullList()
        {
            throw new NotImplementedException();
        }

        public List<IMoneyTransaction> GetReconciliationTransactions(Guid accountUID, DateRange statementDates)
        {
            throw new NotImplementedException();
        }

        public void SaveReconciliation(BankReconciliationDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
