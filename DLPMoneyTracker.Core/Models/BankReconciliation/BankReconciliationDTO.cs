using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.Core.Models.BankReconciliation
{
    public class BankReconciliationDTO
    {
        public IJournalAccount BankAccount { get; set; } = SpecialAccount.InvalidAccount;
        public DateRange StatementDate { get; set; } = new();
        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }

        public void Copy(BankReconciliationDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            this.StatementDate = dto.StatementDate;
            this.StartingBalance = dto.StartingBalance;
            this.EndingBalance = dto.EndingBalance;
        }
    }
}