using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    // TODO: .NET Minimum dates are outside SQL's; make sure the Initial Balance transactions are changed to B-Day
    public class SQLTransactionRepository : ITransactionRepository
    {
        public decimal GetAccountBalanceByMonth(Guid accountUID, int year, int month)
        {
            throw new NotImplementedException();
        }

        public decimal GetAccountBalanceYTD(Guid accountUID, int year)
        {
            throw new NotImplementedException();
        }

        public decimal GetCurrentAccountBalance(Guid accountUID)
        {
            throw new NotImplementedException();
        }

        public void RemoveTransaction(IMoneyTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void SaveTransaction(IMoneyTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public List<IMoneyTransaction> Search(MoneyRecordSearch search)
        {
            throw new NotImplementedException();
        }
    }
}
