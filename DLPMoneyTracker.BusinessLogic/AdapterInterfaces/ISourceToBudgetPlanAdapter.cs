using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.AdapterInterfaces
{
    public interface ISourceToBudgetPlanAdapter : IBudgetPlan
    {
        void ImportSource(BudgetPlan acct);
        void ImportCopy(IBudgetPlan acct);
        void ExportSource(ref BudgetPlan acct);
    }
}
