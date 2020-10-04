using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DLPMoneyTracker.Core
{
    public delegate void SimpleNotification();
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
