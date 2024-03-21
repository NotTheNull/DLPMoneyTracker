using DLPMoneyTracker.Core.Models.BudgetPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.AdapterInterfaces
{
    public interface ISourceToBudgetPlanAdapter<T> : IBudgetPlan, IDLPAdapter<T>
    {

    }
}
