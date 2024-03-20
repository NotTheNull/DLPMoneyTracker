using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    public class SQLBudgetPlanRepository : IBudgetPlanRepository
    {
        public void DeletePlan(Guid planUID)
        {
            throw new NotImplementedException();
        }

        public List<IBudgetPlan> GetFullList()
        {
            throw new NotImplementedException();
        }

        public List<IBudgetPlan> GetUpcomingPlansForAccount(Guid accountUID)
        {
            throw new NotImplementedException();
        }

        public void SavePlan(IBudgetPlan plan)
        {
            throw new NotImplementedException();
        }

        public List<IBudgetPlan> Search(BudgetPlanSearch search)
        {
            throw new NotImplementedException();
        }
    }
}
