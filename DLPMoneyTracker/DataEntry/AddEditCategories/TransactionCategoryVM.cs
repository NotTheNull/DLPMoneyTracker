using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.DataEntry.AddEditCategories
{
    public class TransactionCategoryVM : BaseViewModel, ILinkDataModelToViewModel<TransactionCategory>
    {
		private Guid _id;

		public Guid UID
		{
			get { return _id; }
			set 
			{
				_id = value;
				NotifyPropertyChanged(nameof(this.UID));
			}
		}


		private string _name;

		public string Name
		{
			get { return _name; }
			set 
			{
				_name = value;
				NotifyPropertyChanged(nameof(this.Name));
			}
		}

		private CategoryType _type;

		public CategoryType CategoryType
		{
			get { return _type; }
			set 
			{
				_type = value;
				NotifyPropertyChanged(nameof(this.CategoryType));
			}
		}




		public TransactionCategoryVM() : base()
		{
			this.UID = Guid.NewGuid();
			this.Name = "*NEW*";
			this.CategoryType = CategoryType.NotSet;
		}
		public TransactionCategoryVM(TransactionCategory src) : base()
		{
			this.LoadSource(src);
		}


		public void LoadSource(TransactionCategory src)
		{
			this.UID = src.ID;
			this.Name = src.Name.Trim();
			this.CategoryType = src.CategoryType;
		}

		public TransactionCategory GetSource()
		{
			return new TransactionCategory()
			{
				ID = this.UID,
				Name = this.Name.Trim(),
				CategoryType = this.CategoryType
			};
		}

		public void NotifyAll()
		{
			NotifyPropertyChanged(nameof(this.Name));
			NotifyPropertyChanged(nameof(this.CategoryType));
		}
	}
}
