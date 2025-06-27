namespace DLPMoneyTracker.Core.Models.ScheduleRecurrence
{
    public enum RecurrenceFrequency
    {
        // TODO: Decide whether we should implement these two recurrence types
        //BiWeekly, // every two weeks [26 times a year]
        //SemiMonthly, // twice a month [24 times a year]
        Monthly,

        SemiAnnual,
        Annual
    }

    public interface IScheduleRecurrence
    {
        public const int NOTIFICATION_DAYS_PRIOR = -7;

        DateTime StartDate { get; set; }
        DateTime NextOccurrence { get; }
        DateTime NotificationDate { get; }
        RecurrenceFrequency Frequency { get; }
    }
}