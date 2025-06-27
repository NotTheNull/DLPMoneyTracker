using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;

namespace DLPMoneyTracker.BusinessLogic.Factories
{
    public static class ScheduleRecurrenceFactory
    {
        public static IScheduleRecurrence Default()
        {
            return new MonthlyRecurrence { StartDate = Common.MINIMUM_DATE };
        }

        public static IScheduleRecurrence Build(IScheduleRecurrence copy)
        {
            return Build(copy.Frequency, copy.StartDate);
        }

        public static IScheduleRecurrence Build(RecurrenceFrequency type, DateTime start)
        {
            return type switch
            {
                RecurrenceFrequency.Annual => new AnnualRecurrence() { StartDate = start },
                RecurrenceFrequency.SemiAnnual => new SemiAnnualRecurrence() { StartDate = start },
                RecurrenceFrequency.Monthly => new MonthlyRecurrence() { StartDate = start },
                _ => throw new InvalidOperationException($"Frequency [{type}] is not currently supported"),
            };
        }
    }
}