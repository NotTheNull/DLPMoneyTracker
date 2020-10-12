using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DLPMoneyTracker.DataEntry.AddEditCategories
{
    public class AddEditCategoryVM : BaseViewModel
    {
        private ITrackerConfig _config;
        private TransactionCategoryVM _data;



        public Guid UID
        {
            get { return _data?.UID ?? Guid.Empty; }
            set
            {
                if (_data is null) return;
                _data.UID = value;
                NotifyPropertyChanged(nameof(this.UID));
            }
        }


        public string Name
        {
            get { return _data?.Name ?? string.Empty; }
            set
            {
                if (_data is null) return;
                _data.Name = value;
                NotifyPropertyChanged(nameof(this.Name));
            }
        }


        public CategoryType SelectedCategoryType
        {
            get { return _data?.CategoryType ?? CategoryType.NotSet; }
            set
            {
                if (_data is null) return;
                _data.CategoryType = value;
                NotifyPropertyChanged(nameof(this.SelectedCategoryType));
            }
        }


        public bool ExcludeFromBudget
        {
            get { return _data?.ExcludeFromBudget ?? false; }
            set
            {
                if (_data is null) return;
                _data.ExcludeFromBudget = value;
                NotifyPropertyChanged(nameof(this.ExcludeFromBudget));
            }
        }


        public bool IsEnabled { get { return !(_data is null); } }


        private ObservableCollection<TransactionCategoryVM> _listCategory = new ObservableCollection<TransactionCategoryVM>();
        public ObservableCollection<TransactionCategoryVM> CategoryList { get { return _listCategory; } }


        private List<SpecialDropListItem<CategoryType>> _listCategoryType;
        public List<SpecialDropListItem<CategoryType>> CategoryTypeList { get { return _listCategoryType; } }


        #region Commands
        private RelayCommand _cmdEditMoneyAccount;
        public RelayCommand CommandEditMoneyAccount
        {
            get
            {
                return _cmdEditMoneyAccount ?? (_cmdEditMoneyAccount = new RelayCommand((o) =>
                {
                    if (o is TransactionCategoryVM act)
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
                    if (o is TransactionCategoryVM cat)
                    {
                        this.RemoveCategory(cat);
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
                    _data = new TransactionCategoryVM()
                    {
                        Name = "New"
                    };
                    this.CategoryList.Add(_data);
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


        public AddEditCategoryVM(ITrackerConfig config) : base()
        {
            _config = config;

            _listCategoryType = new List<SpecialDropListItem<CategoryType>>()
            {
                new SpecialDropListItem<CategoryType>("Expense", CategoryType.Expense),
                new SpecialDropListItem<CategoryType>("Income", CategoryType.Income),
                new SpecialDropListItem<CategoryType>("Adjustment", CategoryType.UntrackedAdjustment)
            };

            this.LoadCategories();
        }

        public void Clear()
        {
            _data = null;
            this.NotifyAll();
        }

        public void CommitChanges()
        {
            _config.ClearCategoryList();
            if (this.CategoryList.Any())
            {
                foreach (var c in this.CategoryList)
                {
                    _config.AddCategory(c.GetSource());
                }
                _config.SaveCategories();
            }
        }

        public void DiscardChanges()
        {
            this.LoadCategories();
        }


        public void LoadCategories()
        {
            this.CategoryList.Clear();
            if (_config.CategoryList.Any())
            {
                foreach (var c in _config.CategoryList)
                {
                    this.CategoryList.Add(new TransactionCategoryVM(c));
                }
            }

        }

        private void RemoveCategory(TransactionCategoryVM cat)
        {
            if (cat is null) throw new ArgumentNullException("Category");
            if (this.CategoryList.Contains(cat))
            {
                this.CategoryList.Remove(cat);
            }
        }



        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.UID));
            NotifyPropertyChanged(nameof(this.Name));
            NotifyPropertyChanged(nameof(this.SelectedCategoryType));
            NotifyPropertyChanged(nameof(this.CategoryList));
            NotifyPropertyChanged(nameof(this.IsEnabled));
            NotifyPropertyChanged(nameof(this.ExcludeFromBudget));
        }
    }
}
