namespace MoneyTrackerWebApp.Models.Core
{
    public class StorageService<T>
    {
        public T Data { get; set; }
        public string ReturnURL { get; set; }

    }
}
