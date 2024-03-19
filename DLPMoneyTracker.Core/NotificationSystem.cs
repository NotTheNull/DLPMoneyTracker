﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core
{
    /// <summary>
    /// Purpose is to host the events that the use cases will trigger.
    /// MUST BE A SINGLETON
    /// </summary>
    public class NotificationSystem
    {
        public delegate void TransactionsModifiedHandler(Guid debitAccountId, Guid creditAccountId);
        public event TransactionsModifiedHandler TransactionsModified;
    }
}
