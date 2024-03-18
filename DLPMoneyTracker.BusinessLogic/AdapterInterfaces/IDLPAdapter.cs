using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.AdapterInterfaces
{
    public interface IDLPAdapter<T>
    {
        void ImportSource(T acct);
        void ExportSource(ref T acct);
    }
}
