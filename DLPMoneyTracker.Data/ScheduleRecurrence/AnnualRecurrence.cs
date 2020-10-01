using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLPMoneyTracker.Data.ScheduleRecurrence
{
    public class AnnualRecurrence : IScheduleRecurrence
    {
        public DateTime NextOccurence
        {
            get
            {
                if (this.AnniversaryDate > DateTime.Today) return this.AnniversaryDate;
                return this.AnniversaryDate.AddYears(1);
            }
        }

        public DateTime NotificationDate
        {
            get
            {
                return this.NextOccurence.AddDays(IScheduleRecurrence.NOTIFICATION_DAYS_PRIOR);
            }
        }

        public RecurrenceFrequency Frequency { get { return RecurrenceFrequency.Annual; } }


        public DateTime StartDate { get; set; }

        public DateTime AnniversaryDate 
        {
            get
            {
                return new DateTime(DateTime.Now.Year, this.StartDate.Month, this.StartDate.Day);
            }
        }




        public AnnualRecurrence() 
        {
            this.StartDate = DateTime.Now;
        }
        public AnnualRecurrence(string fileData)
        {
            this.LoadFileData(fileData);
        }




        public string GetFileData()
        {
            return string.Format("{0}|{1}", IScheduleRecurrence.FREQUENCY_ANNUAL, this.StartDate);
        }

        public void LoadFileData(string data)
        {
            var breakdown = data.Split("|");
            if (breakdown is null) return;
            if (breakdown.Count() < 2) return;

            if (DateTime.TryParse(breakdown[1], out DateTime date))
            {
                this.StartDate = date;
            }
        }
    }
}
