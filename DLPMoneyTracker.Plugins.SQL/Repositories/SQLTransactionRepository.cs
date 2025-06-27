using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Plugins.SQL.Adapters;
using Microsoft.EntityFrameworkCore;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    public class SQLTransactionRepository(IDLPConfig config, ILedgerAccountRepository accountRepository) : ITransactionRepository
    {
        private readonly IDLPConfig config = config;
        private readonly ILedgerAccountRepository accountRepository = accountRepository;

        public decimal GetAccountBalanceByMonth(Guid accountUID, int year, int month)
        {
            DateRange searchDate = new(year, month);
            return GetAccountBalance(accountUID, searchDate);
        }

        public decimal GetAccountBalanceYTD(Guid accountUID, int year)
        {
            DateRange searchDate = new(new DateTime(DateTime.Today.Year, 1, 1), DateTime.Now);
            return GetAccountBalance(accountUID, searchDate);
        }

        public decimal GetCurrentAccountBalance(Guid accountUID)
        {
            DateRange searchDate = new(DateTime.Today.Year, DateTime.Today.Month);
            return GetAccountBalance(accountUID, searchDate);
        }

        private decimal GetAccountBalance(Guid accountUID, DateRange searchDate)
        {
            using DataContext context = new(config);
            var account = context.Accounts.FirstOrDefault(x => x.AccountUID == accountUID);
            if (account is null) return decimal.Zero;

            // We don't want to report totals for the Special Accounts
            if (account.AccountType == Core.Models.LedgerAccounts.LedgerType.NotSet) return decimal.Zero;
            if (account.AccountType != Core.Models.LedgerAccounts.LedgerType.Payable && account.AccountType != Core.Models.LedgerAccounts.LedgerType.Receivable)
            {
                // These are REAL accounts and, therefore, we need the sum of full history
                searchDate.Begin = Common.MINIMUM_DATE;
            }

            return context.TransactionDetails
                .Where(x => 
                    x.LedgerAccount != null &&
                    x.LedgerAccount.AccountUID == accountUID &&
                    x.Batch != null &&
                    x.Batch.TransactionDate >= searchDate.Begin &&
                    x.Batch.TransactionDate <= searchDate.End)
                .Sum(s => s.Amount);
        }

        public void RemoveTransaction(IMoneyTransaction transaction)
        {
            using DataContext context = new(config);
            var existingRecord = context.TransactionBatches.FirstOrDefault(x => x.BatchUID == transaction.UID);
            if (existingRecord is null) return;

            context.TransactionBatches.Remove(existingRecord);
            context.SaveChanges();
        }

        public void SaveTransaction(IMoneyTransaction transaction)
        {
            using DataContext context = new(config);
            SQLSourceToTransactionAdapter adapter = new(context, accountRepository);
            adapter.Copy(transaction);

            var existingRecord = context.TransactionBatches.FirstOrDefault(x => x.BatchUID == transaction.UID);
            if (existingRecord is null)
            {
                existingRecord = new Data.TransactionBatch();
                context.TransactionBatches.Add(existingRecord);
            }

            adapter.ExportSource(ref existingRecord);
            context.SaveChanges();
        }

        public List<IMoneyTransaction> GetFullList()
        {
            List<IMoneyTransaction> listMoneyFinal = [];
            using (DataContext context = new(config))
            {
                var listRecords = context.TransactionBatches.Include(x => x.Details).ToList();
                if (listRecords?.Any() != true) return listMoneyFinal;

                SQLSourceToTransactionAdapter adapter = new(context, accountRepository);
                foreach (var record in listRecords)
                {
                    adapter.ImportSource(record);
                    listMoneyFinal.Add(new MoneyTransaction(adapter));
                }
            }
            return listMoneyFinal;
        }

        public List<IMoneyTransaction> Search(MoneyRecordSearch search)
        {
            List<IMoneyTransaction> listMoneyFinal = [];
            using (DataContext context = new(config))
            {
                var listRecords = context.TransactionBatches.Where(x => x.TransactionDate >= search.DateRange.Begin && x.TransactionDate <= search.DateRange.End);

                if (search.Account != null)
                {
                    listRecords = listRecords.Where(x => x.Details.Any(x => x.LedgerAccount != null && x.LedgerAccount.AccountUID == search.Account.Id));
                }

                if (!string.IsNullOrWhiteSpace(search.SearchText))
                {
                    listRecords = listRecords.Where(x => x.Description.Contains(search.SearchText));
                }

                var listRecordsLoop = listRecords.Include(x => x.Details).ToList();
                if (listRecordsLoop?.Any() != true) return listMoneyFinal;

                SQLSourceToTransactionAdapter adapter = new(context, accountRepository);
                foreach (var record in listRecordsLoop)
                {
                    adapter.ImportSource(record);
                    listMoneyFinal.Add(new MoneyTransaction(adapter));
                }
            }

            return listMoneyFinal;
        }

        public long GetRecordCount()
        {
            using DataContext context = new(config);
            return context.TransactionBatches.LongCount();
        }
    }
}