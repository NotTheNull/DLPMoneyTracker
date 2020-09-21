using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;

namespace DLPMoneyTracker.DataEntry.AddEditMoneyAccount
{
    public class AddEditMoneyAccountVM : BaseViewModel, IDisposable
    {
        #region Objects and Properties
        private ITrackerConfig _config;
        private MoneyAccountVM _data;

        public MoneyAccountVM SelectedAccount
        {
            get { return _data; }
            set
            {
                _data = value;
                this.NotifyAll();
            }
        }

        public string ID
        {
            get { return this.SelectedAccount?.ID ?? string.Empty; }
            set
            {
                if (this.SelectedAccount is null) return;
                this.SelectedAccount.ID = value;
                NotifyPropertyChanged(nameof(this.ID));
            }
        }

        public string Description
        {
            get { return this.SelectedAccount?.Description ?? string.Empty; }
            set
            {
                if (this.SelectedAccount is null) return;
                this.SelectedAccount.Description = value;
                NotifyPropertyChanged(nameof(this.Description));
            }
        }

        public MoneyAccountType SelectedAccountType
        {
            get { return this.SelectedAccount?.AccountType ?? MoneyAccountType.NotSet; }
            set
            {
                if (this.SelectedAccount is null) return;
                this.SelectedAccount.AccountType = value;
                NotifyPropertyChanged(nameof(this.SelectedAccountType));
            }
        }

        public string WebAddress
        {
            get { return this.SelectedAccount?.WebAddress ?? string.Empty; }
            set
            {
                if (this.SelectedAccount is null) return;
                this.SelectedAccount.WebAddress = value;
                NotifyPropertyChanged(nameof(this.WebAddress));
            }
        }


        public bool IsEnabled { get { return !(this.SelectedAccount is null); } }




        public ObservableCollection<MoneyAccountVM> MoneyAccountList { get; set; }

        public List<SpecialDropListItem<MoneyAccountType>> AccountTypeList { get; set; }


        #endregion


        #region Commands
        private RelayCommand _cmdEditMoneyAccount;
        public RelayCommand CommandEditMoneyAccount
        {
            get
            {
                return _cmdEditMoneyAccount ?? (_cmdEditMoneyAccount = new RelayCommand((o) =>
                  {
                      if (o is MoneyAccountVM act)
                      {
                          _data = act;
                          this.NotifyAll();
                      }
                  }));
            }
        }

        private RelayCommand _cmdDelMoneyAcct;
        public RelayCommand CommandDeleteMoneyAccount
        {
            get
            {
                return _cmdDelMoneyAcct ?? (_cmdDelMoneyAcct = new RelayCommand((o) =>
                {
                    if (o is MoneyAccountVM act)
                    {
                        this.RemoveAccount(act);
                    }
                }));
            }
        }


        private RelayCommand _cmdClear;
        public RelayCommand CommandClear
        {
            get
            {
                return _cmdClear ?? (_cmdClear = new RelayCommand((o) =>
                {
                    this.Clear();
                }));
            }
        }


        private RelayCommand _cmdAddNew;
        public RelayCommand CommandAddNew
        {
            get
            {
                return _cmdAddNew ?? (_cmdAddNew = new RelayCommand((o) =>
                {
                    _data = new MoneyAccountVM()
                    {
                        ID = "*NEW*",
                        Description = "New"
                    };
                    this.MoneyAccountList.Add(_data);
                    this.NotifyAll();
                }));
            }
        }


        private RelayCommand _cmdSaveChanges;
        public RelayCommand CommandSaveChanges
        {
            get
            {
                return _cmdSaveChanges ?? (_cmdSaveChanges = new RelayCommand((o) =>
                {
                    this.CommitChanges();
                    this.Clear();
                }));
            }
        }


        private RelayCommand _cmdDiscard;
        public RelayCommand CommandDiscardChanges
        {
            get
            {
                return _cmdDiscard ?? (_cmdDiscard = new RelayCommand((o) =>
                {
                    this.DiscardChanges();
                    this.Clear();
                }));
            }
        }

        #endregion


        public AddEditMoneyAccountVM(ITrackerConfig config) : base()
        {
            _config = config;
            this.MoneyAccountList = new ObservableCollection<MoneyAccountVM>();
            this.LoadAccounts();

            this.AccountTypeList = new List<SpecialDropListItem<MoneyAccountType>>
            {
                new SpecialDropListItem<MoneyAccountType>("Checking", MoneyAccountType.Checking),
                new SpecialDropListItem<MoneyAccountType>("Savings", MoneyAccountType.Savings),
                new SpecialDropListItem<MoneyAccountType>("Credit Card", MoneyAccountType.CreditCard),
                new SpecialDropListItem<MoneyAccountType>("Loan", MoneyAccountType.Loan)
            };

            this.Clear();
        }
        ~AddEditMoneyAccountVM() { this.Dispose(); }

        public void DiscardChanges()
        {
            this.LoadAccounts();
        }

        public void LoadAccounts()
        {
            this.MoneyAccountList.Clear();
            if (_config.AccountsList.Any())
            {
                foreach (var act in _config.AccountsList.Where(x => !string.IsNullOrWhiteSpace(x.ID)))
                {
                    this.MoneyAccountList.Add(new MoneyAccountVM(act));
                }
            }
        }

        public void CommitChanges()
        {
            _config.AccountsList.Clear();
            foreach (var acct in this.MoneyAccountList)
            {
                _config.AccountsList.Add(acct.GetSource());
            }

            _config.SaveMoneyAccounts();
        }

        public void RemoveAccount(MoneyAccountVM act)
        {
            if (this.MoneyAccountList.Contains(act)) this.MoneyAccountList.Remove(act);
        }


        public void Clear()
        {
            _data = null;
            this.NotifyAll();
        }


        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.AccountTypeList));
            NotifyPropertyChanged(nameof(this.ID));
            NotifyPropertyChanged(nameof(this.SelectedAccount));
            NotifyPropertyChanged(nameof(this.SelectedAccountType));
            NotifyPropertyChanged(nameof(this.Description));
            NotifyPropertyChanged(nameof(this.WebAddress));
            NotifyPropertyChanged(nameof(this.IsEnabled));
            NotifyPropertyChanged(nameof(this.MoneyAccountList));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (!(this.MoneyAccountList is null))
            {
                foreach (var record in this.MoneyAccountList) record.Dispose();
                this.MoneyAccountList.Clear();
                this.MoneyAccountList = null;
            }

            if(!(this.AccountTypeList is null))
            {
                this.AccountTypeList.Clear();
                this.AccountTypeList = null;
            }

            if(!(_data is null))
            {
                _data.Dispose();
                _data = null;
            }

            _config = null;
        }
    }
}
