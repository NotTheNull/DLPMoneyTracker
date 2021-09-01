using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.Data.Common
{
    public class DateRange
    {
        public readonly DateTime Begin;
        public readonly DateTime End;

        public DateRange(DateTime begin, DateTime end)
        {
            this.Begin = begin;
            this.End = end;
        }
    }
}
