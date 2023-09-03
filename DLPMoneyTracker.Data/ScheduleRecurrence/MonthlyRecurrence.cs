using System;
using System.Linq;

namespace DLPMoneyTracker.Data.ScheduleRecurrence
{
    public class MonthlyRecurrence : IScheduleRecurrence
    {
        public DateTime NextOccurence
        {
            get
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                int day = this.DayOfMonth;
                int numberOfDaysInMonth = DateTime.DaysInMonth(year, month);

                if (day > numberOfDaysInMonth) day = numberOfDaysInMonth;
                // Get current month then check if we're past that date

                DateTime next = new DateTime(year, month, day);
                if (next < DateTime.Today)
                {
                    month++;
                    if (month > 12)
                    {
                        year++;
                        month = 1;
                    }
                    numberOfDaysInMonth = DateTime.DaysInMonth(year, month);
                    if (day > numberOfDaysInMonth) day = numberOfDaysInMonth;
                    next = new DateTime(year, month, day);
                }

                return next;
            }
        }

        public DateTime NotificationDate
        {
            get
            {
                return this.NextOccurence.AddDays(IScheduleRecurrence.NOTIFICATION_DAYS_PRIOR);
            }
        }

        public RecurrenceFrequency Frequency
        { get { return RecurrenceFrequency.Monthly; } }

        private int _dayOfMonth;

        public int DayOfMonth
        {
            get { return _dayOfMonth; }
            set
            {
                if (value < 1) _dayOfMonth = 1;
                else if (value > 31) _dayOfMonth = 31;
                else _dayOfMonth = value;
            }
        }

        public MonthlyRecurrence()
        {
            _dayOfMonth = 1;
        }

        public MonthlyRecurrence(string fileData)
        {
            this.LoadFileData(fileData);
        }

        public string GetFileData()
        {
            return string.Format("{0}|{1}", IScheduleRecurrence.FREQUENCY_MONTHLY, this.DayOfMonth);
        }

        public void LoadFileData(string data)
        {
            var breakdown = data.Split("|");
            if (breakdown is null) return;
            if (breakdown.Count() < 2) return;

            if (int.TryParse(breakdown[1], out int day))
            {
                this.DayOfMonth = day;
            }
            else
            {
                this.DayOfMonth = 1;
            }
        }
    }
}