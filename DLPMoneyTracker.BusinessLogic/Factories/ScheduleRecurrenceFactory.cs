using DLPMoneyTracker.Core.Models.ScheduleRecurrence;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.Factories
{
    public class ScheduleRecurrenceFactory
    {

        public IScheduleRecurrence Build(IScheduleRecurrence copy)
        {
            return Build(copy.Frequency, copy.StartDate);
        }


        public IScheduleRecurrence Build(RecurrenceFrequency type, DateTime start)
        {
            switch(type)
            {
                case RecurrenceFrequency.Annual:
                    return new AnnualRecurrence() { StartDate = start };
                case RecurrenceFrequency.SemiAnnual:
                    return new SemiAnnualRecurrence() { StartDate = start };
                case RecurrenceFrequency.Monthly:
                    return new MonthlyRecurrence() { StartDate = start };
                default:
                    throw new InvalidOperationException($"Frequency [{type}] is not currently supported");
            }
        }
    }
}
