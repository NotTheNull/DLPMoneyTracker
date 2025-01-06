using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System.ComponentModel.DataAnnotations;

namespace MoneyTrackerWebApp.Models.Config.LedgerAccounts
{
    public class EditLedgerAccountVM : INominalAccount
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        [MinLength(3, ErrorMessage = "Description must have at least 3 characters")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public LedgerType JournalType { get; set; } = LedgerType.NotSet;

        public BudgetTrackingType BudgetType { get; set; } = BudgetTrackingType.DO_NOT_TRACK;

        [Range(0.0d, 99999.99d)]
        public decimal DefaultMonthlyBudgetAmount { get; set; } = decimal.Zero;

        public decimal CurrentBudgetAmount { get; } = decimal.Zero;


        [Range(1, 99, ErrorMessage = "Display order must be greater than 1")]
        public int OrderBy { get; set; } = 0;

        public DateTime? DateClosedUTC { get; set; } = null;

        






        public void Copy(IJournalAccount cpy)
        {
            if (cpy is null) return;

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.JournalType = cpy.JournalType;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            
            if(cpy is INominalAccount acc)
            {
                this.BudgetType = acc.BudgetType;
                this.DefaultMonthlyBudgetAmount = acc.DefaultMonthlyBudgetAmount;
                
            }
        }
    }
}
