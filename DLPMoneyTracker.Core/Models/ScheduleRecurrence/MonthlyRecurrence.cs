
using Microsoft.VisualBasic;
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

                DateTime nextTime = this.GetNextDate(DateTime.Today.Month);
                if (DateTime.Today <= nextTime) return nextTime;

                return this.GetNextDate(DateTime.Today.Month + 1);
            }
        }

        public DateTime NotificationDate
        {
            get
            {
                return this.NextOccurrence.AddDays(IScheduleRecurrence.NOTIFICATION_DAYS_PRIOR);
            }
        }

        /// <summary>
        /// Deduces the next start date by verifying whether the Start Date exceeds the number of days
        /// in the given month
        /// </summary>
        /// <param name="month">The month to get the next Start date</param>
        /// <returns>
        /// DateTime representing the next start date
        /// </returns>
        private DateTime GetNextDate(int month)
        {
            int year = DateTime.Today.Year;
            if(DateTime.Today.Month == 12 && month < DateTime.Today.Month)
            {
                // Likely we're going from December to January
                year++;
            }

            DateRange validRange = new DateRange(year, month);
            if (this.StartDate.Day > validRange.End.Day)
            {
                // This is typical for February and months that end in 30
                return new DateTime(year, month, validRange.End.Day);
            }
            else
            {
                return new DateTime(year, month, this.StartDate.Day);
            }
        }

    }
}
