using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.JSON.Adapters;
using DLPMoneyTracker.Plugins.JSON.Models;
using System.Text.Json;

namespace DLPMoneyTracker.Plugins.JSON.Repositories
{
    public class JSONTransactionRepository : ITransactionRepository, IJSONRepository
    {
        private readonly ILedgerAccountRepository accountRepository;
        private readonly IDLPConfig config;
        private readonly int _year;

        public JSONTransactionRepository(ILedgerAccountRepository accountRepository, IDLPConfig config)
        {
            _year = DateTime.Today.Year;
            this.accountRepository = accountRepository;
            this.config = config;
            this.LoadFromFile();
        }

        public List<IMoneyTransaction> TransactionList { get; set; } = [];

        public string FilePath => Path.Combine(config.JSONFilePath, "Data", "Journal.json");

        public void LoadFromFile()
        {
            this.TransactionList.Clear();
            if (!File.Exists(this.FilePath)) return;

            string json = File.ReadAllText(this.FilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<JournalEntryJSON>?)JsonSerializer.Deserialize(json, typeof(List<JournalEntryJSON>));
            if (dataList?.Any() != true) return;

            JSONSourceToTransactionAdapter adapter = new(accountRepository);
            bool reSaveFile = false; // Mainly here to help correct the change for Initial Balance accounts
            foreach (var data in dataList)
            {
                adapter.ImportSource(data);
                reSaveFile = reSaveFile || (adapter.CreditAccountId != data.CreditAccountId || adapter.DebitAccountId != data.DebitAccountId);
                this.TransactionList.Add(new MoneyTransaction(adapter));
            }
        }

        public void SaveToFile()
        {
            if (this.TransactionList.Any() != true) return;

            JSONSourceToTransactionAdapter adapter = new(accountRepository);
            List<JournalEntryJSON> listJSONRecords = [];
            foreach (var t in this.TransactionList)
            {
                adapter.Copy(t);
                JournalEntryJSON jsonRecord = new();
                adapter.ExportSource(ref jsonRecord);
                listJSONRecords.Add(jsonRecord);
            }

            string json = JsonSerializer.Serialize<List<JournalEntryJSON>>(listJSONRecords);
            File.WriteAllText(this.FilePath, json);
        }

        public List<IMoneyTransaction> GetFullList()
        {
            return [.. this.TransactionList];
        }

        public List<IMoneyTransaction> Search(MoneyRecordSearch search)
        {
            var listRecords = this.TransactionList.Where(x => search.DateRange.IsWithinRange(x.TransactionDate));

            if (search.Account != null)
            {
                listRecords = listRecords.Where(x => x.DebitAccountId == search.Account.Id || x.CreditAccountId == search.Account.Id);
            }

            if (!string.IsNullOrWhiteSpace(search.SearchText))
            {
                listRecords = listRecords.Where(x => x.Description.Contains(search.SearchText));
            }

            return [.. listRecords];
        }

        public decimal GetCurrentAccountBalance(Guid accountUID)
        {
            return this.GetAccountBalanceByMonth(accountUID, DateTime.Today.Year, DateTime.Today.Month);
        }

        public decimal GetAccountBalanceByMonth(Guid accountUID, int year, int month)
        {
            IJournalAccount account = accountRepository.GetAccountByUID(accountUID);

            MoneyRecordSearch search = new() { Account = account, DateRange = DateRange.GetMonthRange(year, month) };
            if (account is IMoneyAccount || account is ILiabilityAccount)
            {
                // Assets & Liabilities require the full range of transactions to get the Balance
                search.DateRange.Begin = DateTime.MinValue;
            }
            // Nominal accounts only require the month

            var listRecords = this.Search(search);
            decimal balance = decimal.Zero;
            foreach (var record in listRecords)
            {
                if (record.DebitAccountId == accountUID)
                {
                    balance += record.TransactionAmount;
                }
                else
                {
                    balance -= record.TransactionAmount;
                }
            }

            // NOTE: Do not negate values here, let the UI Handle that
            return balance;
        }

        public decimal GetAccountBalanceYTD(Guid accountUID, int year)
        {
            IJournalAccount account = accountRepository.GetAccountByUID(accountUID);

            MoneyRecordSearch search = new() { Account = account, DateRange = new Core.DateRange(new DateTime(year, 1, 1), new DateTime(year, 12, 31)) };
            if (account is IMoneyAccount)
            {
                // Assets & Liabilities require the full range of transactions to get the Balance
                search.DateRange.Begin = DateTime.MinValue;
            }
            // Nominal accounts only require the month

            var listRecords = this.Search(search);
            decimal balance = decimal.Zero;
            foreach (var record in listRecords)
            {
                if (record.DebitAccountId == accountUID)
                {
                    balance += record.TransactionAmount;
                }
                else
                {
                    balance -= record.TransactionAmount;
                }
            }

            // NOTE: Do not negate values here, let the UI Handle that
            return balance;
        }

        public void RemoveTransaction(IMoneyTransaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            var existingRecord = this.TransactionList.FirstOrDefault(x => x.UID == transaction.UID);
            if (existingRecord is null) return;

            this.TransactionList.Remove(existingRecord);
            this.SaveToFile();
        }

        public void SaveTransaction(IMoneyTransaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            var existingRecord = this.TransactionList.FirstOrDefault(x => x.UID == transaction.UID);
            if (existingRecord is null)
            {
                this.TransactionList.Add(transaction);
            }
            else
            {
                existingRecord.Copy(transaction);
            }

            this.SaveToFile();
        }

        public long GetRecordCount()
        {
            return this.TransactionList.Count;
        }
    }
}