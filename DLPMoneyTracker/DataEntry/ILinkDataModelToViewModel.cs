using System;

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