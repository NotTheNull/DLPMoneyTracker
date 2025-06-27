using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.Core.Models.BankReconciliation
{
    public class BankReconciliationOverviewDTO
    {
        public IJournalAccount BankAccount { get; set; }
        public List<BankReconciliationDTO> ReconciliationList { get; } = [];

        public BankReconciliationDTO? GetRecordForMonth(int month)
        {
            return this.ReconciliationList.FirstOrDefault(x => x.StatementDate.End.Month == month);
        }
    }
}