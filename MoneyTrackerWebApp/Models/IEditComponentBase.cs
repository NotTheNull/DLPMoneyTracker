namespace MoneyTrackerWebApp.Models
{
    public interface IEditComponentBase
    {
        void SaveChanges();
        void Reset();
        void ReturnToList();
    }
}
