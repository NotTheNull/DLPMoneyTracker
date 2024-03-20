using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
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


        public void Copy(IScheduleRecurrence recurrence)
        {
            ArgumentNullException.ThrowIfNull(recurrence);

            this.Frequency = recurrence.Frequency;
            this.StartDate = recurrence.StartDate;
        }

        public void ImportJSON(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;

            // Not exactly JSON but it comes from the JSON file
            // the data is split via "|"
            var breakdown = data.Split("|");
            if (breakdown is null) return;
            if (breakdown.Count() < 2) return;

            switch (breakdown[0])
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

            if (this.Frequency == RecurrenceFrequency.Monthly)
            {
                if (int.TryParse(breakdown[1], out int startDay))
                {
                    this.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, startDay);
                }
                else
                {
                    DateTime.TryParse(breakdown[1], out DateTime newStart);
                    this.StartDate = newStart;
                }
            }
            else
            {
                DateTime.TryParse(breakdown[1], out DateTime newStart);
                this.StartDate = newStart;
            }

        }

        public string ExportJSON()
        {
            string frequency = string.Empty;
            switch(this.Frequency)
            {
                case RecurrenceFrequency.Annual:
                    frequency = FREQUENCY_ANNUAL;
                    break;
                case RecurrenceFrequency.SemiAnnual:
                    frequency = FREQUENCY_SEMI_ANNUAL;
                    break;
                case RecurrenceFrequency.Monthly:
                    frequency = FREQUENCY_MONTHLY;
                    break;
                default:
                    return string.Empty;
            }

            string date = string.Format("{0:yyyy/MM/dd}", this.StartDate);
            return string.Concat(frequency, "|", date);
        }
    }
}
