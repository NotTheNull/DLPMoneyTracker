using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using DLPMoneyTracker.Core.Models.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Adapters
{
    
    public class JSONScheduleRecurrenceAdapter : IScheduleRecurrence
    {
        public const string FREQUENCY_MONTHLY = "MONTHLY";
        public const string FREQUENCY_SEMI_ANNUAL = "SEMIANNUAL";
        public const string FREQUENCY_ANNUAL = "ANNUAL";

        public RecurrenceFrequency Frequency { get; set; } 
        public DateTime StartDate { get; set; }


        // No need to worry about these fields
        public DateTime NextOccurrence => throw new NotImplementedException();

        public DateTime NotificationDate => throw new NotImplementedException();


        public void ImportJSON(string data)
        {
            // Not exactly JSON but it comes from the JSON file
            // the data is split via "|"
            var breakdown = data.Split("|");
            if (breakdown is null) return;
            if (breakdown.Count() < 2) return;

            switch(breakdown[0])
            {
                case FREQUENCY_ANNUAL:
                    Frequency = RecurrenceFrequency.Annual;
                    break;
                case FREQUENCY_SEMI_ANNUAL:
                    Frequency = RecurrenceFrequency.SemiAnnual;
                    break;
                case FREQUENCY_MONTHLY:
                    Frequency = RecurrenceFrequency.Monthly;
                    break;
                default:
                    return;

            }

            if(this.Frequency == RecurrenceFrequency.Monthly)
            {
                int.TryParse(breakdown[1], out int startDay);
                this.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, startDay);
            }
            else
            {
                DateTime.TryParse(breakdown[1], out DateTime newStart);
                this.StartDate = newStart;
            }

        }


    }
}
