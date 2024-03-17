using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.Source
{
    public class TransactionDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TransactionBatch Batch { get; set; }
        public Account LedgerAccount { get; set; }
        public decimal Amount { get; set; }
        public DateTime? BankReconciliationDate { get; set; }

    }
}
