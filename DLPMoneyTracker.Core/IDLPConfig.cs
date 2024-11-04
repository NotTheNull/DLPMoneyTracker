using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core
{
    public enum DLPDataSource
    {
        NotSet,
        JSON,
        Database
    }

    public interface IDLPConfig
    {
        DLPDataSource DataSource { get; }
        string DBConnectionString { get; }
    }
}
