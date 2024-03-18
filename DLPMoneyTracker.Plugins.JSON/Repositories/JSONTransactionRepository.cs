using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core.Models;
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

        private string FolderPath { get { return AppSettings.DATA_FOLDER_PATH.Replace(AppSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }
        public string FilePath { get { return string.Concat(this.FolderPath, "Journal.json"); } }

        public void LoadFromFile()
        {
            this.TransactionList.Clear();
            if (!File.Exists(FilePath)) return;

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<JournalEntryJSON>)JsonSerializer.Deserialize(json, typeof(List<JournalEntryJSON>));
            if (dataList?.Any() != true) return;

            JSONSourceToTransactionAdapter adapter = new JSONSourceToTransactionAdapter(accountRepository);
            foreach(var data in dataList)
            {
                adapter.ImportSource(data);
                this.TransactionList.Add(new MoneyTransaction(adapter));
            }

        }
    }
}
