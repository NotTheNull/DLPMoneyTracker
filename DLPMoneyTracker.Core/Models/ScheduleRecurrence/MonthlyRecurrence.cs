
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.ScheduleRecurrence
{
    public class MonthlyRecurrence : IScheduleRecurrence
    {
        public DateTime StartDate { get; set; } = DateTime.Today;
        public RecurrenceFrequency Frequency { get { return RecurrenceFrequency.Monthly; } }

        public DateTime NextOccurrence
        {
            get
            {
                if (DateTime.Today < this.StartDate) return this.StartDate;

                DateTime nextTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, this.StartDate.Day);
                if (DateTime.Today < nextTime) return nextTime;

                return nextTime.AddMonths(1);
            }
        }

        public DateTime NotificationDate
        {
            get
            {
                return this.NextOccurrence.AddDays(IScheduleRecurrence.NOTIFICATION_DAYS_PRIOR);
            }
        }


    }
}
