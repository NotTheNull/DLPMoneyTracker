using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.PluginInterfaces
{
    public struct BudgetPlanSearch
    {
        public Guid AccountUID;
        public DateRange DateRange;
        public string FilterText;
    }


    public interface IBudgetPlanRepository
    {
        List<IBudgetPlan> Search(BudgetPlanSearch search);
        List<IBudgetPlan> GetUpcomingPlansForAccount(Guid accountUID);
        List<IBudgetPlan> GetFullList();
        List<IBudgetPlan> GetPlanListByType(BudgetPlanType planType);
        void DeletePlan(Guid planUID);
        void SavePlan(IBudgetPlan plan);
        int GetRecordCount();
        List<IBudgetPlan> GetAllPlansForAccount(Guid accountUID);
    }
}
