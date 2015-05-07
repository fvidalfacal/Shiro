// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from Shiro INC. team.
//  
// Copyrights (c) 2014 Shiro INC. All rights reserved.

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