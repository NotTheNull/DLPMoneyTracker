namespace DLPMoneyTracker2.Core
{
    public class SpecialDropListItem<T>
    {
        public string Display { get; set; }
        public T Value { get; set; }

        public SpecialDropListItem(string displayName, T item)
        {
            this.Display = displayName.Trim();
            this.Value = item;
        }
    }
}