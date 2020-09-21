using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DLPMoneyTracker.DataEntry
{
    public interface ILinkDataModelToViewModel<T>
    {
        Guid UID { get; }


        void LoadSource(T src);
        T GetSource();

        void NotifyAll();
    }
}
