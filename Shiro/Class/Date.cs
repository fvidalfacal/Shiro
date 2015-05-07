namespace Shiro.Class
{
    internal static class Date
    {
        internal static string Fr2Us(string date)
        {
            var dateUs = date.Split(new[] {'/'});
            return string.Format("{0}-{1}-{2}",dateUs[2],dateUs[1],dateUs[0]);
        }

        internal static string Us2Fr(string date)
        {
            var dateUs = date.Split(new[] { '-' });
            return string.Format("{0}/{1}/{2}", dateUs[2], dateUs[1], dateUs[0]);
        }
    }
}
