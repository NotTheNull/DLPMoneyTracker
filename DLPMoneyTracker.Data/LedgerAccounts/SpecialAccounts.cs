﻿using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class SpecialAccount : ILedgerAccount
    {
        public static SpecialAccount InitialBalance { get { return new SpecialAccount() { Id = Guid.Empty, Description = "*STARTING BALANCE*" }; } }
        public static SpecialAccount Unlisted { get { return new SpecialAccount() { Id = new Guid("99999999-8888-7777-6666-555555555555"), Description = "*UNDEFINED*" }; } }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerTypes LedgerType { get { return LedgerTypes.NotSet; } }

        public DateTime? DateClosedUTC { get { return null; } }

        public int OrderBy { get { return 0; } }

        public string MoneyAccountId { get { return string.Empty; } }

        public MoneyAccountType AccountType { get { return MoneyAccountType.NotSet; } }

        public Guid CategoryId { get { return Guid.Empty; } }

        public void Copy(ILedgerAccount cpy)
        {
            return;
        }
    }
}
