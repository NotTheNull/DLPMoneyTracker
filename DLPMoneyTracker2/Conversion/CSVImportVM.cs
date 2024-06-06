using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Conversion
{
    public class CSVImportVM : BaseViewModel, IDisposable
    {
        private readonly IGetTransactionsBySearchUseCase searchTransactionUseCase;
        

        public CSVImportVM(IGetTransactionsBySearchUseCase searchTransactionUseCase)
        {
            this.searchTransactionUseCase = searchTransactionUseCase;
        }
        ~CSVImportVM() { this.Dispose(); }


        private IMoneyAccount _account;
        public IMoneyAccount MoneyAccount
        {
            get { return _account; }
            set
            {
                _account = value;
                NotifyPropertyChanged(nameof(MoneyAccount));
            }
        }

        private ICSVMapping Mapping { get { return this.MoneyAccount?.Mapping; } }

        public ObservableCollection<CSVRecordVM> CSVRecords { get; set; } = new ObservableCollection<CSVRecordVM>();



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
                        InitialDirectory = System.IO.Path.Combine(Environment.GetEnvironmentVariable("HOMEPATH"), "Downloads"),
                        Filter = "Comma-Separated Values (*.csv)|*.csv",
                        FilterIndex = 1
                    };
                    
                    fileDialog.ShowDialog();
                    if(string.IsNullOrWhiteSpace(fileDialog.FileName))
                    {
                        this.ImportCSV(fileDialog.FileName);
                    }

                });
            }
        }

        #endregion








        private void ImportCSV(string pathCSV)
        {
            ArgumentNullException.ThrowIfNull(this.MoneyAccount);
            
            this.CSVRecords.Clear();
            if (Mapping is null) return;

            var fileData = System.IO.File.ReadAllText(pathCSV)
                .Split(Environment.NewLine)
                .Select(s => s.Split(","))
                .ToList();

            if(fileData.Count >= this.Mapping.StartingRow)
            {
                for(int i = this.Mapping.StartingRow - 1; i < fileData.Count; i++)
                {
                    this.CSVRecords.Add(new CSVRecordVM
                    {
                        TransactionDate = fileData[i][this.Mapping.GetMapping(ICSVMapping.TRANS_DATE)].ToDateTime(),
                        Description = fileData[i][this.Mapping.GetMapping(ICSVMapping.DESCRIPTION)].Trim(),
                        Amount = fileData[i][this.Mapping.GetMapping(ICSVMapping.AMOUNT)].ToDecimal()
                    });
                }
            }
        }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if(this.CSVRecords != null)
            {
                this.CSVRecords.Clear();
                this.CSVRecords = null;
            }
        }

    }

    public class CSVRecordVM : BaseViewModel
    {

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

        public TransactionVM(IMoneyAccount account)
        {
            this.account = account;
        }








    }
}
