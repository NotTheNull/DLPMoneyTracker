using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.JSON.Adapters;
using DLPMoneyTracker.Plugins.JSON.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace DLPMoneyTracker.Plugins.JSON.Repositories;

public class JSONLedgerAccountRepository : ILedgerAccountRepository, IJSONRepository
{
    private readonly IDLPConfig config;
    private readonly ILogger logger;

    public JSONLedgerAccountRepository(IDLPConfig config, ILogger logger)
    {
        this.config = config;
        this.logger = logger;
        this.LoadFromFile();
    }
    public string FilePath => Path.Combine(config.JSONFilePath, "Config", "LedgerAccounts.json");

    private Dictionary<Guid, IJournalAccount> AccountDataSet { get; } = [];
    private ReadOnlyCollection<IJournalAccount> AccountList => AccountDataSet.Select(s => s.Value).ToList().AsReadOnly();
    private void AddAccount(IJournalAccount account)
    {
        if (AccountDataSet.ContainsKey(account.Id)) return;
        AccountDataSet.Add(account.Id, account);
    }
    private void ClearList() => AccountDataSet.Clear();


    public void LoadFromList(IEnumerable<IJournalAccount> accounts)
    {
        this.ClearList();
        this.AddAccount(SpecialAccount.InitialBalance);
        this.AddAccount(SpecialAccount.UnlistedAdjustment);
        this.AddAccount(SpecialAccount.DebtInterest);
        this.AddAccount(SpecialAccount.DebtReduction);
        foreach (var acct in accounts) this.AddAccount(acct);
    }

    public void LoadFromFile()
    {
        try
        {
            this.ClearList();
            this.AddAccount(SpecialAccount.InitialBalance);
            this.AddAccount(SpecialAccount.UnlistedAdjustment);
            this.AddAccount(SpecialAccount.DebtInterest);
            this.AddAccount(SpecialAccount.DebtReduction);

            if (!File.Exists(FilePath)) return;

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            List<JournalAccountJSON>? dataList = JsonSerializer.Deserialize(json, typeof(List<JournalAccountJSON>)) as List<JournalAccountJSON>;
            if (dataList?.Any() != true) return;

            JSONSourceToJournalAccountAdapter adapter = new(this);
            // Sorting by Summary Account UID so that all the parent accounts are loaded first; this allows the child accounts to load the parent account without error
            foreach (var data in dataList.OrderBy(o => o.SummaryAccountUID)) 
            {
                adapter.ImportSource(data);
                this.AddAccount(JournalAccountFactory.Build(adapter));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to load file");
        }
    }

    public void SaveToFile()
    {
        List<JournalAccountJSON> listJSONData = [];
        JSONSourceToJournalAccountAdapter adapter = new(this);
        foreach (var account in this.AccountList.Where(s => s.JournalType is not LedgerType.Special and not LedgerType.NotSet))
        {
            adapter.Copy(account);

            JournalAccountJSON jsonAccount = new();
            adapter.ExportSource(ref jsonAccount);
            listJSONData.Add(jsonAccount);
        }

        if (listJSONData.Any() != true) return;
        string json = JsonSerializer.Serialize<List<JournalAccountJSON>>(listJSONData);
        File.WriteAllText(this.FilePath, json);
    }

    public IJournalAccount GetAccountByUID(Guid uid) => AccountDataSet[uid];
    
    public List<IJournalAccount> GetFullList() => [.. AccountList];

    public List<IJournalAccount> GetAccountsBySearch(JournalAccountSearch search)
    {
        if (search.JournalTypes.Count == 0) return [];

        var listAccounts = this.AccountList.Where(x => search.JournalTypes.Contains(x.JournalType));
        if (!string.IsNullOrWhiteSpace(search.NameFilterText))
        {
            listAccounts = listAccounts.Where(x => x.Description.Contains(search.NameFilterText));
        }

        if (!search.IncludeDeleted)
        {
            listAccounts = listAccounts.Where(x => !x.DateClosedUTC.HasValue);
        }

        return [.. listAccounts];
    }

    public void SaveJournalAccount(IJournalAccount account)
    {
        ArgumentNullException.ThrowIfNull(account);

        var existingAccount = this.AccountList.FirstOrDefault(x => x.Id == account.Id);
        if (existingAccount is null)
        {
            this.AddAccount(account);
        }
        else
        {
            existingAccount.Copy(account);
        }

        this.SaveToFile();
    }

    public int GetRecordCount() => this.AccountList.Count;

    public List<IJournalAccount> GetSummaryAccountListByType(LedgerType type)
    {
        List<IJournalAccount> listData = [.. this.AccountList.Where(x => x.JournalType == type)];

        List<IJournalAccount> listAccounts = [];
        foreach (var data in listData)
        {
            if (data is ISubLedgerAccount sub)
            {
                if (sub.SummaryAccount is null)
                {
                    listAccounts.Add(data);
                }
            }
        }

        return listAccounts;
    }

    public List<IJournalAccount> GetDetailAccountsForSummary(Guid uidSummaryAccount)
    {
        if (uidSummaryAccount == Guid.Empty) return [];

        List<IJournalAccount> listAccounts = [];
        foreach (var account in this.AccountList)
        {
            if (account is ISubLedgerAccount sub)
            {
                if (sub.SummaryAccount?.Id == uidSummaryAccount)
                {
                    listAccounts.Add(account);
                }
            }
        }

        return listAccounts;
    }

    public Guid GetNextUID()
    {
        Guid next;
        do
        {
            next = Guid.NewGuid();
        } while (AccountDataSet.ContainsKey(next));

        return next;
    }

}