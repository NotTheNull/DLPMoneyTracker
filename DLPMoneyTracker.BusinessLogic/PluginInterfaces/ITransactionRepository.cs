﻿using DLPMoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.PluginInterfaces
{
    public interface ITransactionRepository
    {
        void SaveTransaction(IMoneyTransaction transaction);
    }
}
