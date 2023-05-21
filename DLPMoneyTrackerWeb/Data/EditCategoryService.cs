using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTrackerWeb.Data
{
    internal class EditCategoryService

    {
        private readonly ITrackerConfig _config;

        public EditCategoryService(ITrackerConfig config)
        {
            _config = config;
        }


        public IReadOnlyList<TransactionCategory> Categories { get { return _config.CategoryList; } }



        public TransactionCategory GetCategory(Guid uid)
        {
            
            return this.Categories.FirstOrDefault(x => x.ID == uid);
        }

        public void SaveCategory(TransactionCategory category) 
        { 
            if(!Categories.Contains(category)) 
            {
                _config.AddCategory(category);
            }
            _config.SaveCategories();
        }

        public void ReloadCategories()
        {
            _config.LoadCategories();
        }

        public void DeleteCategory(Guid catId)
        {
            var category = this.Categories.FirstOrDefault(x => x.ID == catId);
            if (category is null) return;

            _config.RemoveCategory(category);
        }
    }
}
