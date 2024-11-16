using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
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
        private readonly IDLPConfig config;

        public JSONLedgerAccountRepository(IDLPConfig config)
        {
            _year = DateTime.Today.Year;
            this.config = config;
            this.LoadFromFile();
        }

        public List<IJournalAccount> AccountList { get; set; } = new List<IJournalAccount>();


        public string FilePath { get { return Path.Combine(config.JSONFilePath, "Config", "LedgerAccounts.json"); } }



        public void LoadFromFile()
        {
            this.AccountList.Clear();
            this.AccountList.Add(SpecialAccount.InitialBalance);
            this.AccountList.Add(SpecialAccount.UnlistedAdjusment);
            this.AccountList.Add(SpecialAccount.DebtInterest);
            this.AccountList.Add(SpecialAccount.DebtReduction);

            string json = string.Empty;
            if (File.Exists(FilePath))
            {
                json = File.ReadAllText(FilePath);
            }
            else
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<JournalAccountJSON>)JsonSerializer.Deserialize(json, typeof(List<JournalAccountJSON>));
            if (dataList?.Any() != true) return;

            JSONSourceToJournalAccountAdapter adapter = new JSONSourceToJournalAccountAdapter(this);
            JournalAccountFactory factory = new JournalAccountFactory();

            foreach (var data in dataList)
            {
                adapter.ImportSource(data);
                this.AccountList.Add(factory.Build(adapter));
            }
        }

        public void SaveToFile()
        {
            List<JournalAccountJSON> listJSONData = new List<JournalAccountJSON>();
            JSONSourceToJournalAccountAdapter adapter = new JSONSourceToJournalAccountAdapter(this);
            foreach (var account in this.AccountList)
            {
                if (account.JournalType == LedgerType.NotSet) continue;
                adapter.Copy(account);
                
                JournalAccountJSON jsonAccount = new JournalAccountJSON();
                adapter.ExportSource(ref jsonAccount);
                listJSONData.Add(jsonAccount);                
            }

            if (listJSONData.Any() != true) return;
            string json = JsonSerializer.Serialize<List<JournalAccountJSON>>(listJSONData);
            File.WriteAllText(this.FilePath, json);
        }






        public IJournalAccount GetAccountByUID(Guid uid)
        {
            return this.AccountList.FirstOrDefault(x => x.Id == uid);
        }

        public List<IJournalAccount> GetFullList()
        {
            return this.AccountList.ToList();
        }

        public List<IJournalAccount> GetAccountsBySearch(JournalAccountSearch search)
        {
            if (search.JournalTypes.Any() != true) return null;

            var listAccounts = this.AccountList.Where(x => search.JournalTypes.Contains(x.JournalType));
            if(!string.IsNullOrWhiteSpace(search.NameFilterText))
            {
                listAccounts = listAccounts.Where(x => x.Description.Contains(search.NameFilterText));
            }

            if(!search.IncludeDeleted)
            {
                listAccounts = listAccounts.Where(x => !x.DateClosedUTC.HasValue);
            }

            return listAccounts.ToList();
        }

        public void SaveJournalAccount(IJournalAccount account)
        {
            ArgumentNullException.ThrowIfNull(account);

            var existingAccount = this.AccountList.FirstOrDefault(x => x.Id == account.Id);
            if(existingAccount is null)
            {
                this.AccountList.Add(account);
            }
            else
            {
                existingAccount.Copy(account);
            }

            this.SaveToFile();
        }

        public int GetRecordCount()
        {
            return this.AccountList.Count;
        }


        public List<IJournalAccount> GetSummaryAccountListByType(LedgerType type)
        {
            var listData = this.AccountList.Where(x => x.JournalType == type).ToList();

            List<IJournalAccount> listAccounts = new List<IJournalAccount>();
            foreach(var data in listData)
            {
                if(data is ISubLedgerAccount sub)
                {
                    if(sub.SummaryAccount is null)
                    {
                        listAccounts.Add(data);
                    }
                }
            }

            return listAccounts;
        }

        public List<IJournalAccount> GetDetailAccountsForSummary(Guid uidSummaryAccount)
        {
            if (uidSummaryAccount == null || uidSummaryAccount == Guid.Empty) return null;

            List<IJournalAccount> listAccounts = new List<IJournalAccount>();
            foreach(var account in this.AccountList)
            {
                if(account is ISubLedgerAccount sub)
                {
                    if(sub.SummaryAccount?.Id == uidSummaryAccount)
                    {
                        listAccounts.Add(account);
                    }
                }
            }

            return listAccounts;
        }

        private readonly Guid[] SPECIAL_UIDS = [SpecialAccount.DebtInterest.Id, SpecialAccount.DebtReduction.Id, SpecialAccount.InitialBalance.Id, SpecialAccount.UnlistedAdjusment.Id, Guid.Empty];

        public Guid GetNextUID()
        {
            Guid next;
            do
            {
                next = Guid.NewGuid();

            } while (DoesUIDExist(next));

            return next;
        }

        private bool DoesUIDExist(Guid uid)
        {
            if (SPECIAL_UIDS.Contains(uid)) return true;
            if (this.AccountList.Select(s => s.Id).Contains(uid)) return true;

            return false;
        }
    }
}
