using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DLPMoneyTracker.Plugins.SQL.Data
{
    public class BankReconciliation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public int BankAccountId { get; set; }
        public DateTime StartingDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndingDate { get; set; } = DateTime.Today;
        public decimal StartingBalance { get; set; } = decimal.Zero;
        public decimal EndingBalance { get; set; } = decimal.Zero;

        public virtual Account? BankAccount { get; set; }
    }
}