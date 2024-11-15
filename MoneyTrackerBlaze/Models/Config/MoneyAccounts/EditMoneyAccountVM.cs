﻿using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTrackerBlaze.Models.Config.MoneyAccounts
{
    internal class EditMoneyAccountVM : IMoneyAccount
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
        public ICSVMapping Mapping { get; set; } = new CSVMapping();

        public void Copy(IJournalAccount cpy)
        {
            if (cpy is null) return;

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.JournalType = cpy.JournalType;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            
            if(cpy is IMoneyAccount money)
            {
                this.Mapping.Copy(money.Mapping);
            }
        }
    }
}
