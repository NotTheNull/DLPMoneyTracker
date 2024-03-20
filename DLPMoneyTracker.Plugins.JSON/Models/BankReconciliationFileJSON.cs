using DLPMoneyTracker.Core.Models.BankReconciliation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Models
{
    internal sealed class BankReconciliationFileJSON
    {
        public Guid AccountId { get; set; }

        public List<BankReconciliationJSON> ReconciliationList { get; set; } = new List<BankReconciliationJSON>();

        public void Copy(BankReconciliationOverviewDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            this.AccountId = dto.BankAccount.Id;

            foreach(var rec in dto.ReconciliationList)
            {
                BankReconciliationJSON json = new BankReconciliationJSON();
                json.Copy(rec);

                this.ReconciliationList.Add(json);
            }
        }
    }
}
