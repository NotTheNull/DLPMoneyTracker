namespace DLPMoneyTracker.BusinessLogic.AdapterInterfaces
{
    public interface IDLPAdapter<T>
    {
        void ImportSource(T src);

        void ExportSource(ref T src);
    }
}