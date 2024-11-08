using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BankReconciliation;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.JSON.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Repositories
{
    public class JSONBankReconciliationRepository : IBankReconciliationRepository, IJSONRepository
    {
        private readonly ILedgerAccountRepository accountRepository;
        private readonly ITransactionRepository moneyRepository;
        private readonly IDLPConfig config;
        private readonly NotificationSystem notification;

        public JSONBankReconciliationRepository(
            ILedgerAccountRepository accountRepository, 
            ITransactionRepository moneyRepository,
            IDLPConfig config,
            NotificationSystem notification)
        {
            this.accountRepository = accountRepository;
            this.moneyRepository = moneyRepository;
            this.config = config;
            this.notification = notification;
            this.LoadFromFile();
        }

        public List<BankReconciliationOverviewDTO> BankReconciliationList { get; set; } = new List<BankReconciliationOverviewDTO>();

        public string FilePath { get { return Path.Combine(config.JSONFilePath, "Reconciliation"); } }



        public void LoadFromFile()
        {
            this.BankReconciliationList.Clear();

            // NOTE: Each Money account will have their own file
            var listBanks = accountRepository.GetAccountsBySearch(JournalAccountSearch.GetMoneyAccounts());
            if (listBanks.Any() != true) return;

            foreach (var bank in listBanks)
            {
                string fileName = string.Format("{0}.json", bank.Id);
                string newPath = Path.Combine(this.FilePath, fileName);

                string json = string.Empty;
                if (File.Exists(newPath))
                {
                    json = File.ReadAllText(newPath);
                }


                if (string.IsNullOrWhiteSpace(json)) continue;

                BankReconciliationFileJSON jsonFile = JsonSerializer.Deserialize<BankReconciliationFileJSON>(json);
                BankReconciliationOverviewDTO bankFile = new BankReconciliationOverviewDTO()
                {
                    BankAccount = bank
                };

                foreach (var rec in jsonFile.ReconciliationList)
                {
                    BankReconciliationDTO bankRec = new BankReconciliationDTO()
                    {
                        BankAccount = bank,
                        StatementDate = new Core.DateRange(rec.StartingDate, rec.EndingDate),
                        StartingBalance = rec.StartingBalance,
                        EndingBalance = rec.EndingBalance
                    };
                    bankFile.ReconciliationList.Add(bankRec);
                }

                this.BankReconciliationList.Add(bankFile);
            }
        }

        public void SaveToFile()
        {
            foreach (var bankFile in this.BankReconciliationList)
            {
                this.SaveToFile(bankFile.BankAccount.Id);
            }
        }

        private void SaveToFile(Guid accountUID)
        {
            var bankFile = this.BankReconciliationList.FirstOrDefault(x => x.BankAccount.Id == accountUID);
            string fileName = string.Format("{0}.json", bankFile.BankAccount.Id);
            string path = Path.Combine(this.FilePath, fileName);

            BankReconciliationFileJSON jsonFile = new BankReconciliationFileJSON();
            jsonFile.Copy(bankFile);

            string json = JsonSerializer.Serialize(jsonFile);
            File.WriteAllText(path, json);
        }

        public List<BankReconciliationOverviewDTO> GetFullList()
        {
            return BankReconciliationList.ToList();
        }

        public List<IMoneyTransaction> GetReconciliationTransactions(Guid accountUID, DateRange statementDates)
        {
            List<IMoneyTransaction> listReconciliation = new List<IMoneyTransaction>();
            
            MoneyRecordSearch search = new MoneyRecordSearch() { Account = accountRepository.GetAccountByUID(accountUID) };
            IEnumerable<IMoneyTransaction> listMoney = moneyRepository.Search(search);

            var listUnreconciled = listMoney.Where(x => (x.DebitAccountId == accountUID && !x.DebitBankDate.HasValue) || (x.CreditAccountId == accountUID && !x.CreditBankDate.HasValue));
            listReconciliation.AddRange(listUnreconciled);

            var listWithBankDates = listMoney
                .Where(x => 
                    (x.DebitAccountId == accountUID && statementDates.IsWithinRange(x.DebitBankDate)) || 
                    (x.CreditAccountId == accountUID && statementDates.IsWithinRange(x.CreditBankDate)));
            listReconciliation.AddRange(listWithBankDates);

            return listReconciliation;
        }

        public void SaveReconciliation(BankReconciliationDTO dto)
        {
            var bankFile = this.BankReconciliationList.FirstOrDefault(x => x.BankAccount.Id == dto.BankAccount.Id);
            var recFile = bankFile.ReconciliationList.FirstOrDefault(x => x.StatementDate == dto.StatementDate);
            if(recFile is null)
            {
                bankFile.ReconciliationList.Add(dto);
            }
            else
            {
                recFile.Copy(dto);
            }

            this.SaveToFile(dto.BankAccount.Id);
            notification.TriggerBankReconciliationChanged(dto.BankAccount.Id);
        }

        public int GetRecordCount()
        {
            return this.BankReconciliationList
                .SelectMany(s => s.ReconciliationList)
                .Count();
        }
    }
}
