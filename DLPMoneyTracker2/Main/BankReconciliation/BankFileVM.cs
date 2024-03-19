
using DLPMoneyTracker.Data.BankReconciliation;

using DLPMoneyTracker2.BankReconciliation;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.BankReconciliation
{
	public class BankFileVM : BaseViewModel
	{
		private readonly ITrackerConfig config;

		public BankFileVM(ITrackerConfig config, IBankReconciliationFile file)
        {
			this.config = config;
			this.LoadFile(file);
		}

		private IJournalAccount account;

		private Guid _uid;

		public Guid AccountId
		{
			get { return _uid; }
			set 
			{ 
				_uid = value;
				NotifyPropertyChanged(nameof(AccountId));
			}
		}

		private string _desc = string.Empty;

		public string AccountDescription
		{
			get { return _desc; }
			set 
			{ 
				_desc = value;
				NotifyPropertyChanged(nameof(AccountDescription));
			}
		}


		private decimal? _initBal = null;

		public decimal? InitialBalance
		{
			get { return _initBal; }
			set 
			{ 
				_initBal = value;
				NotifyPropertyChanged(nameof(InitialBalance));
			}
		}

		private decimal? _janBal = null;

		public decimal? JanuaryBalance
		{
			get { return _janBal; }
			set 
			{ 
				_janBal = value;
				NotifyPropertyChanged(nameof(JanuaryBalance));
			}
		}


		private decimal? _febBal = null;

		public decimal? FebruaryBalance
		{
			get { return _febBal; }
			set 
			{ 
				_febBal = value;
				NotifyPropertyChanged(nameof(FebruaryBalance));
			}
		}


		private decimal? _marBal = null;

		public decimal? MarchBalance
		{
			get { return _marBal; }
			set 
			{
				_marBal = value;
				NotifyPropertyChanged(nameof(MarchBalance));
			}
		}


		private decimal? _aprBal = null;

		public decimal? AprilBalance
		{
			get { return _aprBal; }
			set 
			{ 
				_aprBal = value;
				NotifyPropertyChanged(nameof(AprilBalance));
			}
		}


		private decimal? _mayBal = null;

		public decimal? MayBalance
		{
			get { return _mayBal; }
			set 
			{ 
				_mayBal = value;
				NotifyPropertyChanged(nameof(MayBalance));
			}
		}


		private decimal? _juneBal = null;

		public decimal? JuneBalance
		{
			get { return _juneBal; }
			set 
			{ 
				_juneBal = value;
				NotifyPropertyChanged(nameof(JuneBalance));
			}
		}


		private decimal? _julyBal = null;

		public decimal? JulyBalance
		{
			get { return _julyBal; }
			set 
			{
				_julyBal = value;
				NotifyPropertyChanged(nameof(JulyBalance));
			}
		}


		private decimal? _augBal = null;

		public decimal? AugustBalance
		{
			get { return _augBal; }
			set 
			{
				_augBal = value;
				NotifyPropertyChanged(nameof(AugustBalance));
			}
		}


		private decimal? _septBal = null;

		public decimal? SeptemberBalance
		{
			get { return _septBal; }
			set 
			{
				_septBal = value;
				NotifyPropertyChanged(nameof(SeptemberBalance));
			}
		}


		private decimal? _octBal = null;

		public decimal? OctoberBalance
		{
			get { return _octBal; }
			set 
			{
				_octBal = value;
				NotifyPropertyChanged(nameof(OctoberBalance));
			}
		}


		private decimal? _novBal = null;

		public decimal? NovemberBalance
		{
			get { return _novBal; }
			set 
			{ 
				_novBal = value;
				NotifyPropertyChanged(nameof(NovemberBalance));
			}
		}



		private decimal? _decBal = null;

		public decimal? DecemberBalance
		{
			get { return _decBal; }
			set 
			{ 
				_decBal = value;
				NotifyPropertyChanged(nameof(DecemberBalance));
			}
		}


		private RelayCommand _cmdAddRec;
		public RelayCommand CommandStartNewReconciliation
		{
			get 
			{
				return _cmdAddRec ?? (_cmdAddRec = new RelayCommand((o) =>
				{
					var windowBankRec = UICore.DependencyHost.GetRequiredService<BankReconciliationUI>();
					windowBankRec.LoadAccount(account);
					windowBankRec.Show();
				}));
			}
		}


		public void LoadFile(IBankReconciliationFile file)
		{
			account = config.GetJournalAccount(file.AccountId);
			this.AccountId = account.Id;
			this.AccountDescription = account.Description;
			if (file.ReconciliationList.Any() != true) return;

			var firstRecord = file.ReconciliationList.OrderBy(o => o.EndingDate).FirstOrDefault();
			this.InitialBalance = firstRecord.StartingBalance;

			foreach (var record in file.ReconciliationList.OrderBy(o => o.EndingDate))
			{
				switch(record.EndingDate.Month)
				{
					case 1:
						this.JanuaryBalance = record.EndingBalance;
						break;
					case 2:
						this.FebruaryBalance = record.EndingBalance;
						break;
					case 3:
						this.MarchBalance = record.EndingBalance;
						break;
					case 4:
						this.AprilBalance = record.EndingBalance;
						break;
					case 5:
						this.MayBalance = record.EndingBalance;
						break;
					case 6:
						this.JuneBalance = record.EndingBalance;
						break;
					case 7:
						this.JulyBalance = record.EndingBalance;
						break;
					case 8:
						this.AugustBalance = record.EndingBalance;
						break;
					case 9:
						this.SeptemberBalance = record.EndingBalance;
						break;
					case 10:
						this.OctoberBalance = record.EndingBalance;
						break;
					case 11:
						this.NovemberBalance = record.EndingBalance;
						break;
					case 12:
						this.DecemberBalance = record.EndingBalance;
						break;
				}
			}
		}


	}
}
