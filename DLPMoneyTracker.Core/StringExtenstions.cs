namespace DLPMoneyTracker.Core;

public static class StringExtentions
{
    public static DateTime ToDateTime(this string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return Common.MINIMUM_DATE;
        if (DateTime.TryParse(word, out DateTime result)) return result;

        return Common.MINIMUM_DATE;
    }

    public static int ToInt(this string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return 0;
        if (int.TryParse(word, out int result)) return result;

        return 0;
    }
}