using DLPMoneyTracker.Core.Models.BudgetPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.PluginInterfaces
{
    public interface IBudgetPlanRepository
    {
        List<IBudgetPlan> GetUpcomingPlansForAccount(Guid accountUID);
        List<IBudgetPlan> GetFullList();
        void DeletePlan(Guid planUID);
        void SavePlan(IBudgetPlan plan);
    }
}
