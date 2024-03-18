using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Repositories
{
    public class JSONBankReconciliationRepository : IBankReconciliationRepository, IJSONRepository
    {
        public string FilePath => throw new NotImplementedException();

        public void LoadFromFile()
        {
            throw new NotImplementedException();
        }
    }
}
