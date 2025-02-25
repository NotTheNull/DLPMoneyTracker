namespace MoneyTrackerWebApp.Models.CSVImport
{
    public class CSVRecord
    {
        public Guid UID { get; set; } = Guid.NewGuid();
        public bool IsSelected { get; set; } = false;
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; } = decimal.Zero;

    }
}
