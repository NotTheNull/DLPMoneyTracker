using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker2.LedgerEntry;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DLPMoneyTracker2.Conversion
{
    public class CSVImportVM : BaseViewModel, IDisposable
    {
        private readonly IGetTransactionsBySearchUseCase searchTransactionUseCase;
        private readonly IGetJournalAccountListByTypesUseCase searchAccountsUseCase;
        private readonly ISaveTransactionUseCase saveTransactionUseCase;

        public CSVImportVM(
            IGetTransactionsBySearchUseCase searchTransactionUseCase, 
            IGetJournalAccountListByTypesUseCase searchAccountsUseCase,
            ISaveTransactionUseCase saveTransactionUseCase)
        {
            this.searchTransactionUseCase = searchTransactionUseCase;
            this.searchAccountsUseCase = searchAccountsUseCase;
            this.saveTransactionUseCase = saveTransactionUseCase;
            this.Init();
        }
        ~CSVImportVM() { this.Dispose(); }

        #region Properties


        private string _path = string.Empty;

        public string CSVFilePath
        {
            get { return string.IsNullOrWhiteSpace(_path) ? "* NO FILE *" : _path; }
            set
            {
                _path = value;
                NotifyPropertyChanged(nameof(this.CSVFilePath));
            }
        }



        private DateRange bankDateRange = new DateRange();

        public DateTime StartDate
        {
            get { return bankDateRange.Begin; }
            set
            {
                bankDateRange.Begin = value;
                NotifyPropertyChanged(nameof(this.StartDate));
            }
        }

        public DateTime EndDate
        {
            get { return bankDateRange.End; }
            set
            {
                bankDateRange.End = value;
                NotifyPropertyChanged(nameof(this.EndDate));
            }
        }





        private IMoneyAccount _account;
        public IMoneyAccount SelectedMoneyAccount
        {
            get { return _account; }
            set
            {
                _account = value;
                NotifyPropertyChanged(nameof(SelectedMoneyAccount));

                if(!string.IsNullOrWhiteSpace(this.CSVFilePath))
                {
                    this.ImportCSV();
                    this.PrefillDateRange();
                    this.LoadTransactions();
                }
            }
        }

        public ObservableCollection<SpecialDropListItem<IMoneyAccount>> AccountList { get; set; } = new ObservableCollection<SpecialDropListItem<IMoneyAccount>>();





        private ICSVMapping Mapping { get { return this.SelectedMoneyAccount?.Mapping; } }

        public ObservableCollection<CSVRecordVM> CSVRecordList { get; set; } = new ObservableCollection<CSVRecordVM>();

        public ObservableCollection<TransactionVM> TransactionList { get; set; } = new ObservableCollection<TransactionVM>();

        #endregion

        #region Commands

        private RelayCommand _cmdLoadCSV;

        public RelayCommand CommandLoadCSV
        {
            get
            {
                return _cmdLoadCSV ??= new RelayCommand((o) =>
                {
                    OpenFileDialog fileDialog = new OpenFileDialog()
                    {
                        CheckPathExists = true,
                        CheckFileExists = true,
                        DefaultExt = "csv",
                        InitialDirectory = "C:\\",
                        Filter = "Comma-Separated Values (*.csv)|*.csv",
                        FilterIndex = 1,
                        Multiselect=false
                    };

                    bool? isFileSelected = fileDialog.ShowDialog();
                    if (isFileSelected == true)
                    {
                        this.LoadCSV(fileDialog.FileName);
                    }

                });
            }
        }

        private RelayCommand _cmdRefresh;
        public RelayCommand CommandRefresh
        {
            get
            {
                return _cmdRefresh ??= new RelayCommand((o) =>
                {
                    this.ImportCSV();
                    this.LoadTransactions();
                });
            }
        }

        private RelayCommand _cmdMatch;
        public RelayCommand CommandMatch
        {
            get
            {
                return _cmdMatch ??= new RelayCommand((o) =>
                {
                    this.MatchCSVtoTransactions();
                });
            }
        }


        private RelayCommand _cmdAddNew;
        public RelayCommand CommandAddNewTransaction
        {
            get
            {
                return _cmdAddNew ??= new RelayCommand((o) =>
                {
                    this.CreateNewTransaction();
                });
            }
        }

        private RelayCommand _cmdAutoClear;
        public RelayCommand CommandAutoClear
        {
            get
            {
                return _cmdAutoClear ??= new RelayCommand((o) =>
                {
                    this.AutoClear();
                });
            }
        }


        #endregion



        private void AutoClear()
        {
            var loopList = this.CSVRecordList.ToList();
            foreach(var csv in loopList)
            {
                var transaction = this.TransactionList.FirstOrDefault(x => x.BankDate == csv.TransactionDate && Math.Abs(x.Amount) == Math.Abs(csv.Amount));
                if (transaction is null) continue;

                this.CSVRecordList.Remove(csv);
                this.TransactionList.Remove(transaction);
            }
        }


        private void PrefillDateRange()
        {
            if (this.Mapping is null) return;
            if (this.CSVRecordList?.Any() != true) return;

            // Adding seven days on either end to account for processing delays
            this.StartDate = this.CSVRecordList.OrderBy(o => o.TransactionDate).FirstOrDefault().TransactionDate.AddDays(-7);
            this.EndDate = this.CSVRecordList.OrderBy(o => o.TransactionDate).LastOrDefault().TransactionDate.AddDays(7);
        }



        /// <summary>
        /// Uses the CSV record to build a new transaction
        /// </summary>
        private void CreateNewTransaction()
        {
            // Get CSV record
            var csv = this.CSVRecordList.FirstOrDefault(x => x.IsSelected);
            if (csv is null) return;

            bool isCredit = (csv.Amount < 0 && !this.Mapping.IsAmountInverted);
            MoneyTransaction record = new MoneyTransaction()
            {
                TransactionDate = csv.TransactionDate,
                Description = csv.Description,
                TransactionAmount = Math.Abs(csv.Amount)
            };
            if(isCredit)
            {
                record.CreditAccount = _account;
                record.CreditBankDate = csv.TransactionDate;
                record.JournalEntryType = TransactionType.Expense;
            } 
            else if(this.SelectedMoneyAccount.JournalType == LedgerType.LiabilityCard)
            {
                record.DebitAccount = _account;
                record.DebitBankDate = csv.TransactionDate;
                record.JournalEntryType = TransactionType.DebtPayment;
            } else
            {
                record.DebitAccount = _account;
                record.DebitBankDate = csv.TransactionDate;
                record.JournalEntryType = TransactionType.Income;
            }


            var viewModel = JournalEntryVMFactory.BuildViewModel(record);
            RecordJournalEntry window = new RecordJournalEntry(viewModel);
            window.Show();

            // Remove CSV from Collection list
            this.CSVRecordList.Remove(csv);
        }


        /// <summary>
        /// Pulls the CSV record (should only ever be one) and saves its Date 
        /// to the matched transaction(s) Bank Date field.  Then removes those records from the lists.
        /// </summary>
        private void MatchCSVtoTransactions()
        {
            // Get CSV record
            var csv = this.CSVRecordList.FirstOrDefault(x => x.IsSelected);
            if (csv is null) return;

            // Get matched transactions
            var transactions = this.TransactionList.Where(s => s.IsSelected).ToList();
            if (transactions?.Any() != true) return;

            if(Math.Abs(csv.Amount) != Math.Abs(transactions.Sum(s => s.Amount)))
            {
                MessageBox.Show("Amounts do not match!");
                return;
            }


            // Update Bank Date on transactions
            foreach(var t in transactions)
            {
                t.BankDate = csv.TransactionDate;
                saveTransactionUseCase.Execute(t.Transaction);

                // Remove transactions from Collection list
                this.TransactionList.Remove(t);
            }

            // Remove CSV from Collection list
            this.CSVRecordList.Remove(csv);
        }





        private void Init()
        {
            this.StartDate = DateTime.Today.AddDays(-60);
            this.EndDate = DateTime.Today;


            var results = searchAccountsUseCase.Execute(new List<LedgerType>() { LedgerType.Bank, LedgerType.LiabilityCard });
            if (results?.Any() != true) return;

            foreach (var account in results)
            {
                if (account is IMoneyAccount money)
                {
                    this.AccountList.Add(new SpecialDropListItem<IMoneyAccount>(account.Description, money));
                }
            }
        }

        private List<string[]> csvData = new List<string[]>();

        /// <summary>
        /// Reads the contents of the CSV file into a List
        /// </summary>
        /// <param name="pathCSV"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void LoadCSV(string pathCSV)
        {
            if (string.IsNullOrWhiteSpace(pathCSV)) throw new ArgumentNullException("File path");
            this.CSVFilePath = pathCSV;

            csvData.Clear();
            var fileData = System.IO.File.ReadAllText(pathCSV);
            string splitThis = "\n";
            if(fileData.Contains("\n\r"))
            {
                splitThis = "\n\r";
            } else if(fileData.Contains("\r\n"))
            {
                splitThis = "\r\n";
            }
            var listData = fileData
                .Split(splitThis)
                .Select(s => s.Split(","))
                .ToList();


            if (listData?.Any() == true) csvData.AddRange(listData);
            this.PrefillDateRange();
        }

        /// <summary>
        /// Converts the in-memory CSV data to the Record View Model
        /// </summary>
        private void ImportCSV()
        {
            ArgumentNullException.ThrowIfNull(this.SelectedMoneyAccount);

            this.CSVRecordList.Clear();
            if (Mapping is null) return;
            if (csvData.Count <= this.Mapping.StartingRow) return;


            for (int i = this.Mapping.StartingRow - 1; i < csvData.Count; i++)
            {
                if (csvData[i].Length <= 1)
                {
                    // This is a blank row; skip it
                    continue;
                }

                this.CSVRecordList.Add(new CSVRecordVM
                {
                    TransactionDate = csvData[i][this.Mapping.GetMapping(ICSVMapping.TRANS_DATE)].RemoveQuotes().ToDateTime(),
                    Description = csvData[i][this.Mapping.GetMapping(ICSVMapping.DESCRIPTION)].Trim(),
                    Amount = csvData[i][this.Mapping.GetMapping(ICSVMapping.AMOUNT)].RemoveQuotes().ToDecimal()
                });
            }

        }

        /// <summary>
        /// Loads all transactions whose bank date is within the range OR is null
        /// </summary>
        private void LoadTransactions()
        {
            ArgumentNullException.ThrowIfNull(this.SelectedMoneyAccount);

            this.TransactionList.Clear();

            var results = searchTransactionUseCase.Execute(bankDateRange, null, this.SelectedMoneyAccount);
            if (results?.Any() != true) return;

            foreach (var t in results)
            {
                this.TransactionList.Add(new TransactionVM(this.SelectedMoneyAccount, t));
            }
        }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.CSVRecordList?.Clear();
            this.TransactionList?.Clear();
            csvData?.Clear();
            this.CSVFilePath = string.Empty;
        }

    }

    public class CSVRecordVM : BaseViewModel
    {


        private bool _selected;

        public bool IsSelected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                NotifyPropertyChanged(nameof(this.IsSelected));
            }
        }


        private DateTime _date;

        public DateTime TransactionDate
        {
            get { return _date; }
            set
            {
                _date = value;
                NotifyPropertyChanged(nameof(TransactionDate));
            }
        }

        private string _desc;

        public string Description
        {
            get { return _desc; }
            set
            {
                _desc = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }


        private decimal _amount;

        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                NotifyPropertyChanged(nameof(Amount));
            }
        }

    }

    public class TransactionVM : BaseViewModel
    {
        private readonly IMoneyAccount account;
        private IMoneyTransaction transaction;

        public TransactionVM(IMoneyAccount account, IMoneyTransaction trans)
        {
            this.account = account;
            this.transaction = trans;
        }

        public IMoneyTransaction Transaction { get { return transaction; } }


        private bool _selected;

        public bool IsSelected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                NotifyPropertyChanged(nameof(this.IsSelected));
            }
        }


        // Transaction Date
        public DateTime TransactionDate { get { return transaction.TransactionDate; } }

        // Description
        public string Description { get { return transaction.Description; } }

        // Amount
        public decimal Amount { get { return transaction.TransactionAmount; } }

        //// Other account
        //public string OffsetAccountName
        //{
        //    get
        //    {
        //        if (account.Id == transaction.DebitAccountId) return transaction.CreditAccountName;

        //        return transaction.DebitAccountName;
        //    }
        //}

        // Bank Date
        public DateTime? BankDate
        {
            get
            {
                if (account.Id == transaction.DebitAccountId) return transaction.DebitBankDate;

                return transaction.CreditBankDate;
            }
            set
            {
                if (transaction is MoneyTransaction record)
                {
                    if (account.Id == transaction.DebitAccountId)
                    {
                        record.DebitBankDate = value;
                    }
                    else
                    {
                        record.CreditBankDate = value;
                    }

                }

            }
        }


    }
}
