using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DLPMoneyTracker.DataEntry.AddEditMoneyAccount
{
    public class MoneyAccountVM : BaseViewModel, IDisposable
    {
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



		public MoneyAccountVM() : base() 
		{
			this.ID = this.Description = this.WebAddress = string.Empty;
			this.AccountType = MoneyAccountType.NotSet;
		}
		public MoneyAccountVM(MoneyAccount src) : base()
		{
			this.Load(src);
		}
		~MoneyAccountVM() { this.Dispose(); }

		public void Load(MoneyAccount src)
		{
			if (src is null) throw new ArgumentNullException("Source");
			if (string.IsNullOrWhiteSpace(this.ID)) throw new InvalidOperationException("ID cannot be blank");

			this.ID = src.ID.Trim();
			this.AccountType = src.AccountType;
			this.Description = src.Description?.Trim() ?? string.Empty;
			this.WebAddress = src.WebAddress?.Trim() ?? string.Empty;
		}

		public MoneyAccount GetMoneyAccount()
		{
			if (string.IsNullOrWhiteSpace(this.ID)) throw new InvalidOperationException("ID cannot be blank");

			return new MoneyAccount()
			{
				ID = this.ID.Trim(),
				AccountType = this.AccountType,
				Description = this.Description.Trim(),
				WebAddress = this.WebAddress.Trim()
			};
		}

		public void NotifyAll()
		{
			NotifyPropertyChanged(nameof(this.ID));
			NotifyPropertyChanged(nameof(this.AccountType));
			NotifyPropertyChanged(nameof(this.Description));
			NotifyPropertyChanged(nameof(this.WebAddress));
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.ID = this.Description = this.WebAddress = string.Empty;
			this.AccountType = MoneyAccountType.NotSet;
		}
	}
}
