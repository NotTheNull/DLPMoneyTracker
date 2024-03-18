
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.ScheduleRecurrence
{
    public class SemiAnnualRecurrence : IScheduleRecurrence
    {
        public DateTime StartDate { get; set; } = DateTime.Today;
        public RecurrenceFrequency Frequency { get { return RecurrenceFrequency.SemiAnnual; } }

        public DateTime NextOccurrence
        {
            get
            {
                if (DateTime.Today < this.StartDate) return this.StartDate;

                if (DateTime.Today < this.FirstDate) return this.FirstDate;
                if (DateTime.Today < this.SecondDate) return this.SecondDate;

                return this.SecondDate.AddMonths(6);
            }
        }

        public DateTime NotificationDate => throw new NotImplementedException();


        private DateTime FirstDate { get { return new DateTime(DateTime.Today.Year, this.StartDate.Month, this.StartDate.Day); } }
        private DateTime SecondDate { get { return FirstDate.AddMonths(6); } }

    }
}
