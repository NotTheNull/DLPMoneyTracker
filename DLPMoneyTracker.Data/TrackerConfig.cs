using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DLPMoneyTracker.Data
{
	public delegate void SimpleNotification();
	public struct JournalAccountSearch
    {
        public List<LedgerType> JournalTypes;
        public string NameFilterText;
        public bool IncludeDeleted;

        public JournalAccountSearch()
        {
            JournalTypes = new List<LedgerType>();
            IncludeDeleted = false;
            NameFilterText = string.Empty;
        }

        public JournalAccountSearch(IEnumerable<LedgerType> listAccountTypes)
        {
            if (listAccountTypes is null) throw new ArgumentNullException(nameof(listAccountTypes));

            JournalTypes = new List<LedgerType>();
            JournalTypes.AddRange(listAccountTypes);
            IncludeDeleted = false;
            NameFilterText = string.Empty;
        }

        public static JournalAccountSearch GetMoneyAccounts()
        {
            return new JournalAccountSearch
            {
                JournalTypes = new List<LedgerType> { LedgerType.Bank, LedgerType.LiabilityCard },
                IncludeDeleted = false,
                NameFilterText = string.Empty
            };
        }
    }

    public interface ITrackerConfig : IDisposable
    {
        void LoadFromFile(int year);

        void AddJournalAccount(IJournalAccount act);

        void RemoveJournalAccount(Guid accountId);

        void SaveJournalAccounts();

        void LoadJournalAccounts();

        IJournalAccount GetJournalAccount(Guid accountId);

        IEnumerable<IJournalAccount> GetJournalAccountList(JournalAccountSearch search);

        void Copy(ITrackerConfig config);
    }

    public class TrackerConfig : ITrackerConfig
    {
        private int _year;
        private string FolderPath
        { get { return AppConfigSettings.CONFIG_FOLDER_PATH.Replace(AppConfigSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }

        private string LedgerAccountsConfig
        { get { return string.Concat(this.FolderPath, "LedgerAccounts.json"); } }


        private List<IJournalAccount> _listLedgerAccounts = new List<IJournalAccount>();


        public TrackerConfig() : this(DateTime.Today.Year)
        {
        }

        public TrackerConfig(int year)
        {
            this.LoadFromFile(year);
        }

        ~TrackerConfig()
        {
            this.Dispose();
        }

        public void LoadFromFile(int year)
        {
            _year = year;

            if (!Directory.Exists(this.FolderPath))
            {
                Directory.CreateDirectory(this.FolderPath);
            }

            this.LoadJournalAccounts();
        }

        public void AddJournalAccount(IJournalAccount act)
        {
            if (act is null) throw new ArgumentNullException("Journal Account");
            if (act.JournalType == LedgerType.NotSet) throw new InvalidCastException("Journal Type is NOT SET");
            if (_listLedgerAccounts.Any(x => x.Id == act.Id)) return;
            _listLedgerAccounts.Add(act);
            SaveJournalAccounts();
        }

        public void RemoveJournalAccount(Guid accountId)
        {
            if (accountId == Guid.Empty) throw new ArgumentNullException("Account Id");

            var act = _listLedgerAccounts.FirstOrDefault(x => x.Id == accountId);
            if (act is null) return;
            act.DateClosedUTC = DateTime.UtcNow;
            SaveJournalAccounts();
        }

        public void SaveJournalAccounts()
        {
            string json = JsonSerializer.Serialize(_listLedgerAccounts.Where(x => x.JournalType != LedgerType.NotSet).ToList(), typeof(List<IJournalAccount>));
            File.WriteAllText(LedgerAccountsConfig, json);
        }

        public void LoadJournalAccounts()
        {
            _listLedgerAccounts ??= new List<IJournalAccount>();
            _listLedgerAccounts.Clear();
            _listLedgerAccounts.Add(SpecialAccount.InitialBalance);
            _listLedgerAccounts.Add(SpecialAccount.UnlistedAdjusment);
            _listLedgerAccounts.Add(SpecialAccount.DebtInterest);
            _listLedgerAccounts.Add(SpecialAccount.DebtReduction);

            if (File.Exists(LedgerAccountsConfig))
            {
                string json = File.ReadAllText(LedgerAccountsConfig);
                var dataList = (List<JournalAccountJSON>)JsonSerializer.Deserialize(json, typeof(List<JournalAccountJSON>));
                foreach (var data in dataList)
                {
                    _listLedgerAccounts.Add(JournalAccountFactory.Build(data));
                }
            }
        }

        public IJournalAccount GetJournalAccount(Guid accountId)
        {
            return _listLedgerAccounts.FirstOrDefault(x => x.Id == accountId);
        }

        public IEnumerable<IJournalAccount> GetJournalAccountList(JournalAccountSearch search)
        {
            if (search.JournalTypes.Any() != true) return null;

            var listJournalAccounts = _listLedgerAccounts.Where(x => search.JournalTypes.Contains(x.JournalType));

            if (!string.IsNullOrWhiteSpace(search.NameFilterText))
            {
                listJournalAccounts = listJournalAccounts.Where(x => x.Description.Contains(search.NameFilterText));
            }

            if (!search.IncludeDeleted)
            {
                listJournalAccounts = listJournalAccounts.Where(x => !x.DateClosedUTC.HasValue);
            }

            return listJournalAccounts.ToList();
        }

        public void AddLedgerAccount(IJournalAccount act)
        {
            if (act is null) return;
            if (act.Id == Guid.Empty) return;
            if (_listLedgerAccounts.Any(x => x.Id == act.Id)) return;
            _listLedgerAccounts.Add(act);
        }

        public void Copy(ITrackerConfig config)
        {
            _listLedgerAccounts.Clear();
            JournalAccountSearch search = new JournalAccountSearch();
            search.JournalTypes.AddRange(new List<LedgerType>() { LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan, LedgerType.Payable, LedgerType.Receivable });
            var listPreviousAccounts = config.GetJournalAccountList(search);

            foreach (var gl in listPreviousAccounts)
            {
                this.AddLedgerAccount(gl);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_listLedgerAccounts is not null)
            {
                _listLedgerAccounts.Clear();
                _listLedgerAccounts = null;
            }
        }
    }
}