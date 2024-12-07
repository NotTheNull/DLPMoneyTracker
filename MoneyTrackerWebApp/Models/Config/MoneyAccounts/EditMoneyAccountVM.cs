using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace MoneyTrackerWebApp.Models.Config.MoneyAccounts
{
    public class EditMoneyAccountVM : IMoneyAccount
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        [MinLength(3, ErrorMessage = "Description must have at least 3 characters")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public LedgerType JournalType { get; set; } = LedgerType.NotSet;

        [Range(1, 99, ErrorMessage = "Display order must be greater than 1")]
        public int OrderBy { get; set; } = 0;

        public DateTime? DateClosedUTC { get; set; } = null;

        #region CSV Mapping
        public ICSVMapping Mapping { get; set; } = new CSVMapping();
        private CSVMapping WriteMapping { get { return Mapping as CSVMapping; } }

        public int StartingRow
        {
            get { return Mapping.StartingRow; }
            set { WriteMapping.StartingRow = value; }
        }
        public bool IsAmountInverted
        {
            get { return Mapping.IsAmountInverted; }
            set { WriteMapping.IsAmountInverted = value; }
        }
        public int TransDateColumn
        {
            get { return Mapping.GetMapping(ICSVMapping.TRANS_DATE); }
            set { Mapping.SetMapping(ICSVMapping.TRANS_DATE, value); }
        }
        public int DescriptionColumn
        {
            get { return Mapping.GetMapping(ICSVMapping.DESCRIPTION); }
            set { Mapping.SetMapping(ICSVMapping.DESCRIPTION, value); }
        }
        public int AmountColumn
        {
            get { return Mapping.GetMapping(ICSVMapping.AMOUNT); }
            set { Mapping.SetMapping(ICSVMapping.AMOUNT, value); }
        }

        #endregion








        public void Copy(IJournalAccount cpy)
        {
            if (cpy is null) return;

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.JournalType = cpy.JournalType;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;

            if (cpy is IMoneyAccount money)
            {
                this.Mapping.Copy(money.Mapping);
            }
        }
    }
}
