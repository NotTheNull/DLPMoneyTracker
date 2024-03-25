using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.JSON.Adapters;
using DLPMoneyTracker.Plugins.JSON.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Repositories
{
    public class JSONTransactionRepository : ITransactionRepository, IJSONRepository
    {
        private readonly ILedgerAccountRepository accountRepository;
        private int _year;

        public JSONTransactionRepository(ILedgerAccountRepository accountRepository)
        {
            _year = DateTime.Today.Year;
            this.accountRepository = accountRepository;

            this.LoadFromFile();
        }

        public List<IMoneyTransaction> TransactionList { get; set; } = new List<IMoneyTransaction>();

        private string OldFolderPath { get { return AppSettings.OLD_DATA_FOLDER_PATH.Replace(AppSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }
        public string OldFilePath { get { return Path.Combine(this.OldFolderPath, "Journal.json"); } }
        public string FilePath { get { return Path.Combine(AppSettings.NEW_DATA_FOLDER_PATH, "Journal.json"); } }


        public void LoadFromFile()
        {
            this.TransactionList.Clear();
            string json = string.Empty;
            if(File.Exists(this.FilePath))
            {
                json = File.ReadAllText(this.FilePath);
            }
            else if(File.Exists(this.OldFilePath)) 
            {
                if(!Directory.Exists(AppSettings.NEW_DATA_FOLDER_PATH))
                {
                    Directory.CreateDirectory(AppSettings.NEW_DATA_FOLDER_PATH);
                }

                json = File.ReadAllText(this.OldFilePath);
                File.WriteAllText(this.FilePath, json);
            }
            else
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<JournalEntryJSON>)JsonSerializer.Deserialize(json, typeof(List<JournalEntryJSON>));
            if (dataList?.Any() != true) return;

            JSONSourceToTransactionAdapter adapter = new JSONSourceToTransactionAdapter(accountRepository);
            bool reSaveFile = false; // Mainly here to help correct the change for Initial Balance accounts
            foreach(var data in dataList)
            {
                adapter.ImportSource(data);
                reSaveFile = reSaveFile || ( adapter.CreditAccountId != data.CreditAccountId || adapter.DebitAccountId != data.DebitAccountId);
                this.TransactionList.Add(new MoneyTransaction(adapter));
            }

            // TODO: Remove this after updating accounts accordingly
            if(reSaveFile)
            {
                this.SaveToFile();
            }
        }

        public void SaveToFile()
        {
            if (this.TransactionList.Any() != true) return;

            JSONSourceToTransactionAdapter adapter = new JSONSourceToTransactionAdapter(accountRepository);
            List<JournalEntryJSON> listJSONRecords = new List<JournalEntryJSON>();
            foreach(var t in this.TransactionList)
            {
                adapter.Copy(t);
                JournalEntryJSON jsonRecord = new JournalEntryJSON();
                adapter.ExportSource(ref jsonRecord);
                listJSONRecords.Add(jsonRecord);
            }

            string json = JsonSerializer.Serialize<List<JournalEntryJSON>>(listJSONRecords);
            File.WriteAllText(this.FilePath, json);
        }


        public List<IMoneyTransaction> GetFullList()
        {
            return this.TransactionList.ToList();
        }

        public List<IMoneyTransaction> Search(MoneyRecordSearch search)
        {
            var listRecords = this.TransactionList.Where(x => search.DateRange.IsWithinRange(x.TransactionDate));

            if(search.Account != null)
            {
                listRecords = listRecords.Where(x => x.DebitAccountId == search.Account.Id || x.CreditAccountId == search.Account.Id);
            }

            if(!string.IsNullOrWhiteSpace(search.SearchText))
            {
                listRecords = listRecords.Where(x => x.Description.Contains(search.SearchText));
            }

            return listRecords.ToList();
        }

        public decimal GetCurrentAccountBalance(Guid accountUID)
        {
            return this.GetAccountBalanceByMonth(accountUID, DateTime.Today.Year, DateTime.Today.Month);
        }

        public decimal GetAccountBalanceByMonth(Guid accountUID, int year, int month)
        {
            IJournalAccount account = accountRepository.GetAccountByUID(accountUID);

            MoneyRecordSearch search = new MoneyRecordSearch { Account = account, DateRange = new Core.DateRange(year, month) };
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
                if(record.DebitAccountId == accountUID)
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

            MoneyRecordSearch search = new MoneyRecordSearch { Account = account, DateRange = new Core.DateRange(new DateTime(year, 1, 1), new DateTime(year, 12, 31)) };
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
            if(existingRecord is null)
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
