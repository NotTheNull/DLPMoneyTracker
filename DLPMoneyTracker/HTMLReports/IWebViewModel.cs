using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.HTMLReports
{
    public interface IWebViewModel
    {
        int SelectedMonth { get; }
        
        string HTMLSource { get; }

        void BuildHTML(); 
    }
}
