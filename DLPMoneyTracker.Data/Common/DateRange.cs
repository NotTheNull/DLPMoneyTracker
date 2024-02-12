using System;
using System.Reflection.Metadata;

namespace DLPMoneyTracker.Data.Common
{
    public class DateRange
    {
        public DateTime Begin;
        public DateTime End;

        public DateRange()
        {
            this.Begin = DateTime.Today;
            this.End = DateTime.Today;
        }

        /// <summary>
        /// Sets the date range to the given values
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public DateRange(DateTime begin, DateTime end)
        {
            this.Begin = begin;
            this.End = end;
        }

        /// <summary>
        /// Sets the Date Range for the full month of the given year
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        public DateRange(int year, int month)
        {
            this.Begin = new DateTime(year, month, 1);
            this.End = new DateTime(year, month, DateTime.DaysInMonth(year, month)).AddDays(1).AddMilliseconds(-1);
        }

        public bool IsWithinRange(DateTime date)
        {
            return date >= Begin && date <= End;
        }

        public bool IsWithinRange(DateTime? date)
        {
            if (date.HasValue) return IsWithinRange(date.Value);
            
            return false;
        }
    }
}