using DLPMoneyTracker.Core;

namespace MoneyTrackerWebApp.Models.CSVImport
{
    public class CSVFileDTO
    {
        public List<CSVRecord> RecordList { get; } = new List<CSVRecord>();

        public DateRange Dates
        {
            get
            {
                if (RecordList.Any() == true) return new DateRange(StartDate, EndDate);

                return new DateRange();
            }
        }

        public DateTime StartDate
        {
            get
            {
                return RecordList?.OrderBy(o => o.TransactionDate)?.FirstOrDefault()?.TransactionDate ?? DateTime.MinValue;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return RecordList?.OrderByDescending(o => o.TransactionDate)?.FirstOrDefault()?.TransactionDate ?? DateTime.MaxValue;
            }
        }
    }
}
