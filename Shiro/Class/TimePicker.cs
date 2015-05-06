
namespace Shiro.Class
{
    public static class TimePicker
    {
        public static void AddTime(int temps, System.Windows.Controls.ComboBox comboBoxMinutes)
        {
            for(var i = 0; i < temps; i++)
            {
                comboBoxMinutes.Items.Add(i);
            }
        }
    }
}
