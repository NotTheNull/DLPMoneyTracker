using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLPMoneyTracker.Data.ScheduleRecurrence
{
    public class ScheduleRecurrenceFactory
    {
        public static IScheduleRecurrence Build(string fileData)
        {
            if (string.IsNullOrWhiteSpace(fileData)) throw new InvalidOperationException("Cannot Build Recurrence object without file data");

            var breakdown = fileData.Split("|");
            if (breakdown is null) return null;
            if (breakdown.Count() < 2) return null;

            switch(breakdown[0])
            {
                case IScheduleRecurrence.FREQUENCY_ANNUAL:
                    return new AnnualRecurrence(fileData);
                case IScheduleRecurrence.FREQUENCY_MONTHLY:
                    return new MonthlyRecurrence(fileData);
                case IScheduleRecurrence.FREQUENCY_SEMI_ANNUAL:
                    return new SemiAnnualRecurrence(fileData);
            }

            return null;
        }
    }
}
