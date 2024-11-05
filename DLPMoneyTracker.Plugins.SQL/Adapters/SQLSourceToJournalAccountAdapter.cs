using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Data;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Adapters
{
    public class SQLSourceToJournalAccountAdapter : ISourceToJournalAccountAdapter<Account>
    {
        private readonly ILedgerAccountRepository accountRepository;

        public SQLSourceToJournalAccountAdapter(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType { get; set; }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        public BudgetTrackingType BudgetType { get; set; } = BudgetTrackingType.DO_NOT_TRACK;
        public decimal DefaultMonthlyBudgetAmount { get; set; } = decimal.Zero;
        public decimal CurrentBudgetAmount { get; set; } = decimal.Zero;

        public IJournalAccount SummaryAccount { get; set; }

        public ICSVMapping Mapping { get; set; } = null;

        public void Copy(IJournalAccount cpy)
        {
            ArgumentNullException.ThrowIfNull(cpy);

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.JournalType = cpy.JournalType;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;

            if (cpy is INominalAccount nominal)
            {
                this.BudgetType = nominal.BudgetType;
                this.DefaultMonthlyBudgetAmount = nominal.DefaultMonthlyBudgetAmount;
                this.CurrentBudgetAmount = nominal.CurrentBudgetAmount;
            }
            else if (cpy is IMoneyAccount money)
            {
                if (this.Mapping is null) this.Mapping = new CSVMapping();

                this.Mapping.Copy(money.Mapping);
            }
        }

        public void ExportSource(ref Account acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            acct.AccountUID = this.Id;
            acct.Description = this.Description;
            acct.AccountType = this.JournalType;
            acct.MainTabSortingId = this.OrderBy;
            acct.DateClosedUTC = this.DateClosedUTC;
            acct.BudgetType = this.BudgetType;
            acct.DefaultBudget = this.DefaultMonthlyBudgetAmount;
            acct.CurrentBudget = this.CurrentBudgetAmount;

            if (this.Mapping?.Mapping.Count > 0)
            {
                if (acct.CSVMapping is null)
                {
                    acct.CSVMapping = new CSVMain()
                    {
                        AccountId = acct.Id
                    };
                }
                acct.CSVMapping.IsAmountInverted = this.Mapping.IsAmountInverted;
                acct.CSVMapping.StartingRow = this.Mapping.StartingRow;

                foreach(var key in this.Mapping.Mapping.Keys)
                {
                    var column = acct.CSVMapping.Columns.FirstOrDefault(x => x.ColumnName == key);
                    if(column is null)
                    {
                        column = new CSVColumn()
                        {
                            MainId = acct.CSVMapping.Id,
                            ColumnName = key
                        };
                        acct.CSVMapping.Columns.Add(column);
                    }
                    column.ColumnIndex = this.Mapping.Mapping[key];
                    
                }
            }
        }


        public void ImportSource(Account acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            this.Id = acct.AccountUID;
            this.Description = acct.Description;
            this.JournalType = acct.AccountType;
            this.OrderBy = acct.MainTabSortingId;
            this.DateClosedUTC = acct.DateClosedUTC;
            this.BudgetType = acct.BudgetType;
            this.DefaultMonthlyBudgetAmount = acct.DefaultBudget;
            this.CurrentBudgetAmount = acct.CurrentBudget;

            this.Mapping = null;
            if(acct.CSVMapping != null)
            {
                this.Mapping = new CSVMapping()
                {
                    IsAmountInverted = acct.CSVMapping.IsAmountInverted,
                    StartingRow = acct.CSVMapping.StartingRow
                };

                foreach(var col in acct.CSVMapping.Columns)
                {
                    if(this.Mapping.Mapping.ContainsKey(col.ColumnName))
                    {
                        this.Mapping.Mapping[col.ColumnName] = col.ColumnIndex;
                    } else
                    {
                        this.Mapping.Mapping.Add(col.ColumnName, col.ColumnIndex);
                    }
                }
            }
        }


    }
}
