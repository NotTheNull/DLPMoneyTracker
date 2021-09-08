using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace DLPMoneyTracker.DataEntry.AddEditMoneyAccount
{
    public class MoneyAccountVM : BaseViewModel, IDisposable, ILinkDataModelToViewModel<MoneyAccount>
    {
        public Guid UID { get; set; }

        private string _id;

        [StringLength(10)]
        public string ID
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged(nameof(this.ID));
            }
        }

        private string _desc;

        [StringLength(50)]
        public string Description
        {
            get { return _desc; }
            set
            {
                _desc = value;
                NotifyPropertyChanged(nameof(this.Description));
            }
        }

        private MoneyAccountType _acctType;

        public MoneyAccountType AccountType
        {
            get { return _acctType; }
            set
            {
                _acctType = value;
                NotifyPropertyChanged(nameof(this.AccountType));
            }
        }

        private string _url;

        public string WebAddress
        {
            get { return _url; }
            set
            {
                _url = value;
                NotifyPropertyChanged(nameof(this.WebAddress));
            }
        }

        private decimal _amt;

        public decimal InitialAmount
        {
            get { return _amt; }
            set
            {
                _amt = value;
                NotifyPropertyChanged(nameof(this.InitialAmount));
            }
        }

        private DateTime? _dateClosed;

        public DateTime? DateClosedUTC
        {
            get { return _dateClosed; }
            set 
            {
                _dateClosed = value;
                NotifyPropertyChanged(nameof(this.DateClosedUTC));
            }
        }


        public MoneyAccountVM() : base()
        {
            this.ID = this.Description = this.WebAddress = string.Empty;
            this.AccountType = MoneyAccountType.NotSet;
        }

        public MoneyAccountVM(MoneyAccount src) : base()
        {
            this.LoadSource(src);
        }

        ~MoneyAccountVM()
        {
            this.Dispose();
        }

        public void LoadSource(MoneyAccount src)
        {
            if (src is null) throw new ArgumentNullException("Source");
            if (string.IsNullOrWhiteSpace(src.ID)) throw new InvalidOperationException("ID cannot be blank");

            this.ID = src.ID.Trim();
            this.AccountType = src.AccountType;
            this.Description = src.Description?.Trim() ?? string.Empty;
            this.WebAddress = src.WebAddress?.Trim() ?? string.Empty;
            this.DateClosedUTC = src.DateClosedUTC;
        }

        public MoneyAccount GetSource()
        {
            if (string.IsNullOrWhiteSpace(this.ID)) throw new InvalidOperationException("ID cannot be blank");

            return new MoneyAccount()
            {
                ID = this.ID.Trim(),
                AccountType = this.AccountType,
                Description = this.Description.Trim(),
                WebAddress = this.WebAddress.Trim(),
                DateClosedUTC = this.DateClosedUTC
            };
        }

        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.ID));
            NotifyPropertyChanged(nameof(this.AccountType));
            NotifyPropertyChanged(nameof(this.Description));
            NotifyPropertyChanged(nameof(this.WebAddress));
            NotifyPropertyChanged(nameof(this.InitialAmount));
            NotifyPropertyChanged(nameof(this.DateClosedUTC));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.ID = this.Description = this.WebAddress = string.Empty;
            this.AccountType = MoneyAccountType.NotSet;
        }
    }
}