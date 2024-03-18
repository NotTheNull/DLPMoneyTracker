using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Repositories
{
    internal interface IJSONRepository
    {
        string FilePath { get; }
        void LoadFromFile();
    }
}
