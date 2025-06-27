using DLPMoneyTracker.Core.Models.BankReconciliation;

namespace DLPMoneyTracker.Plugins.JSON.Models
{
    internal sealed class BankReconciliationFileJSON
    {
        public Guid AccountId { get; set; }

        public List<BankReconciliationJSON> ReconciliationList { get; set; } = [];

        public void Copy(BankReconciliationOverviewDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            this.AccountId = dto.BankAccount.Id;

            foreach (var rec in dto.ReconciliationList)
            {
                BankReconciliationJSON json = new();
                json.Copy(rec);

                this.ReconciliationList.Add(json);
            }
        }
    }
}