namespace DLPMoneyTracker.Core.Models.ScheduleRecurrence
{
    public class AnnualRecurrence : IScheduleRecurrence
    {
        public DateTime StartDate { get; set; } = DateTime.Today;
        public RecurrenceFrequency Frequency => RecurrenceFrequency.Annual;

        public DateTime NextOccurrence
        {
            get
            {
                if (DateTime.Today < this.StartDate) return this.StartDate;

                DateTime nextTime = new(DateTime.Today.Year, StartDate.Month, StartDate.Day);
                if (DateTime.Today < nextTime) return nextTime;

                return nextTime.AddYears(1);
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