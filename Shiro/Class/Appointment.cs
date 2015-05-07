// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from Shiro INC. team.
//  
// Copyrights (c) 2014 Shiro INC. All rights reserved.

using System.Windows.Controls;

namespace Shiro.Class
{
    internal sealed class Appointment
    {
        public Appointment(int id, int idCustomer, int idSalesman, string day, string starttime, string endtime)
        {
            IdAppointment = id;
            IdCustomer = idCustomer;
            IdSalesman = idSalesman;
            Day = day;
            Starttime = starttime;
            Endtime = endtime;
        }

        public int IdCustomer { get; set; }
        public int IdAppointment { get; set; }
        public int IdSalesman { get; set; }
        public string Day { get; set; }
        public string Starttime { get; set; }
        public string Endtime { get; set; }
        public Border Border { get; set; }
    }
}