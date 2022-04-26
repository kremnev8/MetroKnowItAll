namespace Util
{
    public static class StringExtension
    {
        public static string FilterRuString(string value)
        {
            value = value.Replace('ё', 'е');
            return value;
        }
    }
}