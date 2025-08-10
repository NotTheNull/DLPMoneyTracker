using System.Reflection.Metadata.Ecma335;

namespace DLPMoneyTracker.Core
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
        public DateRange(DateTime begin, DateTime end) => GetRange(begin, end);

        /// <summary>
        /// Sets the Date Range for the full month of the given year
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        public DateRange(int year, int month) => GetMonthRange(year, month);

        public bool IsWithinRange(DateTime date)
        {
            return date >= Begin && date <= End;
        }

        public bool IsWithinRange(DateTime? date)
        {
            if (date.HasValue) return IsWithinRange(date.Value);

            return false;
        }

        public static DateRange GetMonthRange(int year, int month) => new()
        {
            Begin = new DateTime(year, month, 1),
            End = new DateTime(year, month, DateTime.DaysInMonth(year, month)).AddDays(1).AddMilliseconds(-1)
        };

        public static DateRange GetRange(DateTime begin, DateTime end) => new() { Begin = begin, End = end };
    }
}