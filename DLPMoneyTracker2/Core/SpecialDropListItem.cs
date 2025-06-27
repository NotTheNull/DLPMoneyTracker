namespace DLPMoneyTracker2.Core
{
    public class SpecialDropListItem<T>(string displayName, T item)
    {
        public string Display { get; set; } = displayName.Trim();
        public T Value { get; set; } = item;
    }
}