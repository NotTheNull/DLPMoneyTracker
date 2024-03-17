using DLPMoneyTracker.Core.Models.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.ScheduleRecurrence
{
    public interface IScheduleRecurrence
    {
        public const int NOTIFICATION_DAYS_PRIOR = -7;
        public const string FREQUENCY_MONTHLY = "MONTHLY";
        public const string FREQUENCY_SEMI_ANNUAL = "SEMIANNUAL";
        public const string FREQUENCY_ANNUAL = "ANNUAL";

        DateTime StartDate { get; set; }
        DateTime NextOccurrence { get; }
        DateTime NotificationDate { get; }
        RecurrenceFrequency Frequency { get; }

    }
}
