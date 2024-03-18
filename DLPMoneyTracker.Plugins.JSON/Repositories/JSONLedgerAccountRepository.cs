using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
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
    public class JSONLedgerAccountRepository : ILedgerAccountRepository, IJSONRepository
    {
        private int _year;

        public JSONLedgerAccountRepository()
        {
            _year = DateTime.Today.Year;
            this.LoadFromFile();
        }

        public List<IJournalAccount> AccountList { get; set; } = new List<IJournalAccount>();


        private string FolderPath { get { return AppSettings.CONFIG_FOLDER_PATH.Replace(AppSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }

        public string FilePath { get { return string.Concat(this.FolderPath, "LedgerAccounts.json"); } }




        public void LoadFromFile()
        {
            this.AccountList.Clear();
            this.AccountList.Add(SpecialAccount.InitialBalance);
            this.AccountList.Add(SpecialAccount.UnlistedAdjusment);
            this.AccountList.Add(SpecialAccount.DebtInterest);
            this.AccountList.Add(SpecialAccount.DebtReduction);

            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                if (string.IsNullOrWhiteSpace(json)) return;

                var dataList = (List<JournalAccountJSON>)JsonSerializer.Deserialize(json, typeof(List<JournalAccountJSON>));
                if (dataList?.Any() != true) return;

                JSONSourceToJournalAccountAdapter adapter = new JSONSourceToJournalAccountAdapter();
                JournalAccountFactory factory = new JournalAccountFactory();

                foreach (var data in dataList)
                {
                    adapter.ImportSource(data);
                    this.AccountList.Add(factory.Build(adapter));
                }
            }
        }





        public IJournalAccount GetAccountByUID(Guid uid)
        {
            return this.AccountList.FirstOrDefault(x => x.Id == uid);
        }
    }
}
