using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DLPMoneyTracker.Data
{
    public struct JournalAccountSearch
    {
        public List<JournalAccountType> JournalTypes;
        public string NameFilterText;
        public bool IncludeDeleted;

        public JournalAccountSearch()
        {
            JournalTypes = new List<JournalAccountType>();
            IncludeDeleted = false;
            NameFilterText = string.Empty;
        }

        public JournalAccountSearch(IEnumerable<JournalAccountType> listAccountTypes)
        {
            if (listAccountTypes is null) throw new ArgumentNullException(nameof(listAccountTypes));

            JournalTypes = new List<JournalAccountType>();
            JournalTypes.AddRange(listAccountTypes);
            IncludeDeleted = false;
            NameFilterText = string.Empty;
        }

        public static JournalAccountSearch GetMoneyAccounts()
        {
            return new JournalAccountSearch
            {
                JournalTypes = new List<JournalAccountType> { JournalAccountType.Bank, JournalAccountType.LiabilityCard },
                IncludeDeleted = false,
                NameFilterText = string.Empty
            };
        }
    }

    public interface ITrackerConfig : IDisposable
    {
        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //ReadOnlyCollection<MoneyAccount> AccountsList { get; }
        //[Obsolete]
        //ReadOnlyCollection<TransactionCategory> CategoryList { get; }

        //ReadOnlyCollection<IJournalAccount> LedgerAccountsList { get; }

        void LoadFromFile(int year);

        void AddJournalAccount(IJournalAccount act);

        void RemoveJournalAccount(Guid accountId);

        void SaveJournalAccounts();

        void LoadJournalAccounts();

        IJournalAccount GetJournalAccount(Guid accountId);

        IEnumerable<IJournalAccount> GetJournalAccountList(JournalAccountSearch search);

        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //void LoadMoneyAccounts();

        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //void SaveMoneyAccounts();

        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //void LoadCategories();

        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //void SaveCategories();

        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //TransactionCategory GetCategory(Guid uid);

        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //MoneyAccount GetAccount(string id);

        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //void AddCategory(TransactionCategory cat);

        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //void AddMoneyAccount(MoneyAccount act);

        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //void RemoveCategory(TransactionCategory cat);

        //[Obsolete]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //void RemoveMoneyAccount(MoneyAccount act);

        void Copy(ITrackerConfig config);

        void Convert();
    }

    public class TrackerConfig : ITrackerConfig
    {
        private int _year;
        private string FolderPath
        { get { return AppConfigSettings.CONFIG_FOLDER_PATH.Replace(AppConfigSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }

        private string LedgerAccountsConfig
        { get { return string.Concat(this.FolderPath, "LedgerAccounts.json"); } }

        //#region Obsolete Objects

        //private string AccountListConfig { get { return string.Concat(this.FolderPath, "MoneyAccounts.json"); } }

        //[Obsolete("Switch to ILedger Accounts")]
        //private List<MoneyAccount> _listAccts = new List<MoneyAccount>();
        //[Obsolete("Switch to ILedger Accounts")]
        //public ReadOnlyCollection<MoneyAccount> AccountsList { get { return _listAccts.OrderBy(o => o.ID).ToList().AsReadOnly(); } }

        //private string CategoryListConfig { get { return string.Concat(this.FolderPath, "Categories.json"); } }

        //[Obsolete("Switch to ILedger Accounts")]
        //private List<TransactionCategory> _listCategories = new List<TransactionCategory>();
        //[Obsolete("Switch to ILedger Accounts")]
        //public ReadOnlyCollection<TransactionCategory> CategoryList { get { return _listCategories.OrderBy(o => o.Name).ToList().AsReadOnly(); } }
        //#endregion

        private List<IJournalAccount> _listLedgerAccounts = new List<IJournalAccount>();

        //public ReadOnlyCollection<IJournalAccount> LedgerAccountsList { get { return _listLedgerAccounts.AsReadOnly(); } }

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

            //#pragma warning disable CS0612 // Type or member is obsolete
            //            // Will have to keep these until the conversion is done
            //            this.LoadMoneyAccounts();
            //            this.LoadCategories();
            //#pragma warning restore CS0612 // Type or member is obsolete
            this.LoadJournalAccounts();
        }

        public void AddJournalAccount(IJournalAccount act)
        {
            if (act is null) throw new ArgumentNullException("Journal Account");
            if (act.JournalType == JournalAccountType.NotSet) throw new InvalidCastException("Journal Type is NOT SET");
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
            string json = JsonSerializer.Serialize(_listLedgerAccounts.Where(x => x.JournalType != JournalAccountType.NotSet).ToList(), typeof(List<IJournalAccount>));
            File.WriteAllText(LedgerAccountsConfig, json);
        }

        public void LoadJournalAccounts()
        {
            _listLedgerAccounts ??= new List<IJournalAccount>();
            _listLedgerAccounts.Clear();
            _listLedgerAccounts.Add(SpecialAccount.InitialBalance);
            _listLedgerAccounts.Add(SpecialAccount.UnlistedAdjusment);

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

#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete

        /// <summary>
        /// Converts Money Accounts and Transaction Categories into Ledger Accounts
        /// </summary>
        public void Convert()
        {
            // Conversion is complete
            return;

            //if (_listAccts?.Any() == true)
            //{
            //    var loopAccounts = _listAccts.ToList();
            //    foreach (var act in loopAccounts)
            //    {
            //        // If the account already exists, still need to remove the legacy
            //        if (!LedgerAccountsList.Any(x => x.AccountType != MoneyAccountType.NotSet && x.MoneyAccountId == act.ID))
            //        {
            //            switch (act.AccountType)
            //            {
            //                case MoneyAccountType.Checking:
            //                case MoneyAccountType.Savings:
            //                    _listLedgerAccounts.Add(new BankAccount(act));
            //                    break;
            //                case MoneyAccountType.CreditCard:
            //                    _listLedgerAccounts.Add(new CreditCardAccount(act));
            //                    break;
            //                case MoneyAccountType.Loan:
            //                    _listLedgerAccounts.Add(new LoanAccount(act));
            //                    break;
            //            }

            //        }

            //        // Do Not remove accounts until the Ledger can be converted
            //        //_listAccts.Remove(act);
            //    }

            //    SaveMoneyAccounts();
            //}

            //if (_listCategories?.Any() == true)
            //{
            //    var loopCategories = _listCategories.ToList();
            //    foreach(var cat in loopCategories)
            //    {
            //        // If the category already exists, still need to remove the legacy
            //        if(!LedgerAccountsList.Any(x => x.AccountType == MoneyAccountType.NotSet && x.CategoryId == cat.ID))
            //        {
            //            switch(cat.CategoryType)
            //            {
            //                case CategoryType.Expense:
            //                    PayableAccount ap = new PayableAccount(cat);
            //                    _listLedgerAccounts.Add(ap);
            //                    break;
            //                case CategoryType.Income:
            //                    ReceivableAccount ar = new ReceivableAccount(cat);
            //                    _listLedgerAccounts.Add(ar);
            //                    break;
            //                case CategoryType.UntrackedAdjustment:
            //                    PayableAccount pay = new PayableAccount(cat);
            //                    pay.Description = string.Format("AP {0}", pay.Description);
            //                    _listLedgerAccounts.Add(pay);

            //                    ReceivableAccount rec = new ReceivableAccount(cat);
            //                    rec.Description = string.Format("AR {0}", pay.Description);
            //                    _listLedgerAccounts.Add(rec);
            //                    break;
            //            }

            //        }
            //        // Do Not remove accounts until the Ledger can be converted
            //        //_listCategories.Remove(cat);
            //    }
            //    SaveCategories();
            //}
            //SaveJournalAccounts();
        }

#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete

        public void Copy(ITrackerConfig config)
        {
            //#pragma warning disable CS0618 // Type or member is obsolete
            //#pragma warning disable CS0612 // Type or member is obsolete
            //            // Until the conversion is done, this will need to stayu
            //            _listAccts.Clear();
            //            foreach (var act in config.AccountsList)
            //            {
            //                this.AddMoneyAccount(act);
            //            }

            //            _listCategories.Clear();
            //            foreach (var cat in config.CategoryList)
            //            {
            //                this.AddCategory(cat);
            //            }
            //#pragma warning restore CS0612 // Type or member is obsolete
            //#pragma warning restore CS0618 // Type or member is obsolete

            _listLedgerAccounts.Clear();
            JournalAccountSearch search = new JournalAccountSearch();
            search.JournalTypes.AddRange(new List<JournalAccountType>() { JournalAccountType.Bank, JournalAccountType.LiabilityCard, JournalAccountType.LiabilityLoan, JournalAccountType.Payable, JournalAccountType.Receivable });
            var listPreviousAccounts = config.GetJournalAccountList(search);

            foreach (var gl in listPreviousAccounts)
            {
                this.AddLedgerAccount(gl);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            //#pragma warning disable CS0618 // Type or member is obsolete
            //            // Will need to keep until conversion is done
            //            if (_listAccts is not null)
            //            {
            //                _listAccts.Clear();
            //                _listAccts = null;
            //            }

            //            if (_listCategories is not null)
            //            {
            //                _listCategories.Clear();
            //                _listCategories = null;
            //            }
            //#pragma warning restore CS0618 // Type or member is obsolete

            if (_listLedgerAccounts is not null)
            {
                _listLedgerAccounts.Clear();
                _listLedgerAccounts = null;
            }
        }
    }
}