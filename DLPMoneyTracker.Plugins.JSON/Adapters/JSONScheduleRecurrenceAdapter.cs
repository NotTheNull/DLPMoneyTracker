using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;

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
            if (breakdown.Length < 2) return;

            Frequency = breakdown[0] switch
            {
                FREQUENCY_ANNUAL => RecurrenceFrequency.Annual,
                FREQUENCY_SEMI_ANNUAL => RecurrenceFrequency.SemiAnnual,
                FREQUENCY_MONTHLY => RecurrenceFrequency.Monthly,
                _ => throw new InvalidOperationException($"Recurrence {breakdown[0]} is not supported")
            };

            if (this.Frequency == RecurrenceFrequency.Monthly)
            {
                if (int.TryParse(breakdown[1], out int startDay))
                {
                    DateRange validDates = new(DateTime.Today.Year, DateTime.Today.Month);
                    if (validDates.End.Day < startDay)
                    {
                        // Likely the case for February or months that end with 30 instead of 31
                        startDay = validDates.End.Day;
                    }

                    this.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, startDay);
                }
                else
                {
                    if (DateTime.TryParse(breakdown[1], out DateTime newStart)) this.StartDate = newStart;
                }
            }
            else
            {
                if (DateTime.TryParse(breakdown[1], out DateTime newStart)) this.StartDate = newStart;
            }
        }

        public string ExportJSON()
        {
            string frequency = this.Frequency switch
            {
                RecurrenceFrequency.Annual => FREQUENCY_ANNUAL,
                RecurrenceFrequency.Monthly => FREQUENCY_MONTHLY,
                RecurrenceFrequency.SemiAnnual => FREQUENCY_SEMI_ANNUAL,
                _ => string.Empty
            };

            string date = string.Format("{0:yyyy/MM/dd}", this.StartDate);
            return string.Concat(frequency, "|", date);
        }
    }
}